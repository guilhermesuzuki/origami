using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Text.Json;
using System.Transactions;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class GoogleController : Controller
    {
        protected readonly Serilog.ILogger _logger;
        protected readonly IUserFacade _userFacade;
        protected readonly IMemoryCache _memoryCache;
        protected readonly SocialNetwork _socialNetwork;
        protected readonly ISocialProfileRepository _socialProfile;

        protected readonly string _url = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";
        protected readonly string _refreshUrl = "https://oauth2.googleapis.com/token";

        protected readonly IEventRepository _eventRepository;

        public GoogleController(
            IEventRepository eventRepository,
            ISocialProfileRepository socialProfile,
            Serilog.ILogger logger,
            IUserFacade userFacade,
            IMemoryCache memoryCache,
            IOptions<SocialNetwork> socialNetworkOptions)
            : base()
        {
            _socialProfile = socialProfile;
            _logger = logger;
            _userFacade = userFacade;
            _memoryCache = memoryCache;
            _socialNetwork = socialNetworkOptions.Value;
            _eventRepository = eventRepository;
        }

        [HttpGet("token-ok")]
        public GoogleUserTokenInfo TokenOk([FromQuery] string userId, [FromQuery] string accessToken)
        {
            if (accessToken.Has() == true)
            {
                var url = string.Format(_url, accessToken);

                try
                {
                    using var client = new HttpClient();
                    using var response = client.GetAsync(url).Result;

                    response.EnsureSuccessStatusCode();

                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    if (responseBody.Has() == true)
                    {
                        var tokenInfo = JsonSerializer.Deserialize<GoogleUserTokenInfo>(responseBody);
                        if (tokenInfo?.Has() == true && tokenInfo?.Sub == userId) return tokenInfo;
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.Error(e, $"Validating the Google Token (Status Code {e.StatusCode}): an exception just happened. Here is the token itself: {accessToken}");
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Validating the Google Token: an exception just happened. Here is the token itself: {accessToken}");
                }

                //tries to refresh the token

                try
                {
                    var dict = new Dictionary<string, string>();
                    dict.Add("grant_type", "refresh_token");
                    dict.Add("client_id", _socialNetwork.Google.ClientId);
                    dict.Add("client_secret", _socialNetwork.Google.ClientSecret);
                    dict.Add("refresh_token", accessToken);

                    using var client = new HttpClient();
                    using var request = new HttpRequestMessage(HttpMethod.Post, _refreshUrl) { Content = new FormUrlEncodedContent(dict) };
                    using var response = client.SendAsync(request).Result;

                    response.EnsureSuccessStatusCode();

                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    if (responseBody.Has() == true)
                    {
                        var tokenInfo = JsonSerializer.Deserialize<GoogleUserTokenInfoRefresh>(responseBody);
                        if (tokenInfo?.IdToken.Has() == true)
                        {
                            url = string.Format(_url, tokenInfo.IdToken);

                            try
                            {
                                var newresponse = client.GetAsync(url).Result;
                                newresponse.EnsureSuccessStatusCode();

                                var newresponseBody = newresponse.Content.ReadAsStringAsync().Result;
                                if (newresponseBody.Has() == true)
                                {
                                    var newtokenInfo = JsonSerializer.Deserialize<GoogleUserTokenInfo>(newresponseBody);
                                    if (newtokenInfo?.Has() == true && newtokenInfo?.Sub == userId) return newtokenInfo;
                                }
                            }
                            catch (HttpRequestException e)
                            {
                                _logger.Error(e, $"Validating the Google Token (Status Code {e.StatusCode}): an exception just happened. Here is the token itself: {accessToken}");
                            }
                            catch (Exception e)
                            {
                                _logger.Error(e, $"Validating the Google Token: an exception just happened. Here is the token itself: {accessToken}");
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.Error(e, $"Validating the Google Token (Status Code {e.StatusCode}): an exception just happened. Here is the token itself: {accessToken}");
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Validating the Google Token: an exception just happened. Here is the token itself: {accessToken}");
                }
            }

            return new GoogleUserTokenInfo();
        }

        [AllowAnonymous]
        [HttpGet("get-user")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUser([FromQuery] string userId, [FromQuery] string accessToken, [FromQuery] string returnUrl)
        {
            _logger.Information("Parameters -> userId: {0}, accessToken: {1}", userId, accessToken);

            var ok = TokenOk(userId, accessToken);
            if (ok.Has() == true)
            {
                //looks the user up in the database
                var user = _socialProfile
                    .ReadFromCache()
                    .FirstOrDefault(x => x.SocialNetwork == SocialNetworks.Google && x.UserId == ok.Sub);

                if (user != null && user.IsBlocked)
                {
                    //needs to log the user out, because the facebook user couldn't be found
                    HttpContext.SignOutAsync().GetAwaiter().GetResult();
                    HttpContext.Logout_Workaround();
                    //redirects to the returnUrl with an error
                    return Redirect("/oops/google".QueryString("error", "User has been Blocked"));
                }

                //user doesn't exist in the database, must create a new instance
                if (user == null) user = new OrigamiSocialProfile { SocialNetwork = SocialNetworks.Google, UserId = ok.Sub };

                user.EmailFromSocialNetwork = ok.Email;
                user.FirstName = ok.GivenName;
                user.LastName = ok.FamilyName;
                user.ProfilePictureUrl = ok.Picture.Has() ? ok.Picture : null;

                var context = new DataOperationContext<OrigamiSocialProfile>(OrigamiUser.AnonymousUser, DateTime.UtcNow, user);

                //saves the user into the database
                using (var transaction = new TransactionScope())
                {
                    user = _socialProfile.SmartSave(context, false).Entity;
                    transaction.Complete();
                }

                _userFacade.SocialProfile = user ?? new();
                _eventRepository.SocialProfileLogsIntoWebsite(context.Entity.Id);

                return Redirect(Uri.UnescapeDataString(returnUrl));
            }

            //needs to log the user out, because the facebook user couldn't be found
            HttpContext.SignOutAsync().GetAwaiter().GetResult();
            HttpContext.Logout_Workaround();

            //redirects to the returnUrl with an error
            return Redirect("/oops/google".QueryString("error", "Invalid Google Token"));
        }
    }
}
