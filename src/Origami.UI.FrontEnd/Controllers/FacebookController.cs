using Facebook;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.UI;
using System.Text;
using System.Text.Json.Nodes;
using System.Transactions;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class FacebookController : Controller
    {
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IContentCommentReactionRepository _contentCommentReactionRepository;
        protected readonly IContentCommentRepository _contentCommentRepository;
        protected readonly IContentRatingRepository _contentRatingRepository;
        protected readonly ISocialProfileDeleteRepository _socialProfileForDeletion;
        protected readonly ISocialProfileRepository _socialProfile;
        protected readonly IUserFacade _userFacade;
        protected readonly Serilog.ILogger _logger;
        protected readonly SocialNetwork _socialNetwork;

        protected readonly IEventRepository _eventRepository;

        public FacebookController(
            ISocialProfileRepository socialProfile,
            Serilog.ILogger logger,
            IUserFacade userFacade,
            IMemoryCache memoryCache,
            IOptions<SocialNetwork> socialNetworkOptions,
            IContentCommentReactionRepository contentCommentReactionRepository,
            IContentCommentRepository contentCommentRepository,
            IContentRatingRepository contentRatingRepository,
            ISocialProfileDeleteRepository facebookUserForDeletionRepository,
            IHttpContextAccessor httpContextAccessor,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IEventRepository eventRepository
            ) : base()
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _memoryCache = memoryCache;
            _dbContextFactory = dbContextFactory;
            _contentCommentReactionRepository = contentCommentReactionRepository;
            _contentCommentRepository = contentCommentRepository;
            _contentRatingRepository = contentRatingRepository;
            _socialNetwork = socialNetworkOptions.Value;
            _socialProfile = socialProfile;
            _socialProfileForDeletion = facebookUserForDeletionRepository;
            _userFacade = userFacade;
            _eventRepository = eventRepository;
        }

        /// <summary>
        /// Generates the App Token for making calls to the Facebook API.
        /// </summary>
        protected string AppToken
        {
            get
            {
                //cache key
                var key = "cache-facebook-apptoken";

                if (_memoryCache.TryGetValue(key, out string? value) == true)
                {
                    if (value.Has() == true) return value!;
                }

                var FB = new FacebookClient
                {
                    AppId = _socialNetwork.Facebook.AppId,
                    AppSecret = _socialNetwork.Facebook.AppSecret,
                };

                //parameters for getting an app access token
                var p = new { client_id = FB.AppId, client_secret = FB.AppSecret, grant_type = "client_credentials" };
                var app = FB.Get("/oauth/access_token", p) as dynamic;

                //app access token retrieved
                if (app?.access_token != null)
                {
                    //ATTENTION: APP TOKEN NEVER EXPIRES
                    var token = (string)app.access_token;

                    /*adds app acces token back to cache (renewed this time)*/
                    _memoryCache.Set(key, token);

                    //and returns the generated token
                    return token;
                }

                throw new InvalidOperationException("Could not generate the Application Token from the Facebook API.");
            }
        }

        [AllowAnonymous]
        [HttpPost("delete-user")]
        public IActionResult DeleteUser()
        {
            string? signed_request = Request.Form["signed_request"];

            if (!string.IsNullOrEmpty(signed_request))
            {
                var split = signed_request.Split('.');

                if (string.IsNullOrWhiteSpace(split[0]) == false)
                {
                    int mod4 = split[0].Length % 4;
                    if (mod4 > 0) split[0] += new string('=', 4 - mod4);
                    split[0] = split[0].Replace('-', '+').Replace('_', '/');
                }

                if (string.IsNullOrWhiteSpace(split[1]) == false)
                {
                    int mod4 = split[1].Length % 4;
                    if (mod4 > 0) split[1] += new string('=', 4 - mod4);
                    split[1] = split[1].Replace('-', '+').Replace('_', '/');
                }

                var dataRaw = Encoding.UTF8.GetString(Convert.FromBase64String(split[1]));

                // JSON object
                var json = JsonNode.Parse(dataRaw);

                var appSecretBytes = Encoding.UTF8.GetBytes(_socialNetwork.Facebook.AppSecret);
                var hmac = new System.Security.Cryptography.HMACSHA256(appSecretBytes);
                var expectedHash = Convert.ToBase64String(hmac.ComputeHash(
                    Encoding.UTF8.GetBytes(signed_request.Split('.')[1])))
                    .Replace('-', '+')
                    .Replace('_', '/');

                if (expectedHash != split[0])
                {
                    return BadRequest();
                }

                //*********************
                //Delete your data here
                //*********************

                var userId = json?["user_id"]?.GetValue<string>();

                using var db = _dbContextFactory.CreateDbContext();

                //facebook social profile
                var facebookUser = db.Set<OrigamiSocialProfile>().AsNoTracking()
                    .Where(x => x.SocialNetwork == SocialNetworks.Facebook)
                    .Where(x => x.UserId == userId)
                    .FirstOrDefault();

                if (facebookUser != null)
                {
                    var context = new DataOperationContext<OrigamiSocialProfile>(_userFacade.User!, DateTime.UtcNow, facebookUser);
                    var result = _socialProfileForDeletion.WipeDataOut(context, false);
                    if (result.Ok == true)
                    {
                        return Json(new
                        {
                            url = $"{Request.Scheme}://{Request.Host}/socialprofiles/{result.Entity?.SocialProfileId}",
                            confirmation_code = result.Entity?.Id.ToString(),
                        });
                    }

                    return BadRequest(result.Messages.Error());
                }
            }

            //bad request
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("get-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUser([FromQuery] string userId, [FromQuery] string accessToken, [FromQuery] string returnUrl)
        {
            _logger.Information("Parameters -> userId: {0}, accessToken: {1}", userId, accessToken);

            var ok = TokenOk(userId, accessToken);
            if (ok)
            {
                //facebook client
                var FB = new FacebookClient
                {
                    AppId = _socialNetwork.Facebook.AppId,
                    AppSecret = _socialNetwork.Facebook.AppSecret,
                    AccessToken = accessToken,
                };

                /*second: searches for user profile information*/
                dynamic me = FB.Get("/me?fields=id,email,link,picture,gender,first_name,last_name,cover");
                dynamic? pc = me != null && me?.picture != null ? me?.picture : null;

                /*tries to retrieve the cover photo of the profile*/
                dynamic albums = FB.Get("/me/albums");
                dynamic? cover = null;

                if (albums?.data != null)
                {
                    foreach (var album in albums?.data!)
                    {
                        if (string.Equals(album.name, "cover photos", StringComparison.CurrentCultureIgnoreCase) == true)
                        {
                            dynamic photos = FB.Get($"/{album.id}/photos");
                            if (photos != null && photos?.data != null)
                            {
                                cover = FB.Get($"/{photos?.data[0].id}?fields=link,images");
                                if (cover != null && cover?.images != null) cover = cover?.images[0];
                            }
                        }
                    }
                }

                //looks the user up in the database
                var user = _socialProfile
                    .ReadFromCache()
                    .FirstOrDefault(x => x.SocialNetwork == SocialNetworks.Facebook && x.UserId.Like(userId));

                if (user != null && user.IsBlocked)
                {
                    //needs to log the user out, because the facebook user couldn't be found
                    HttpContext.SignOutAsync().GetAwaiter().GetResult();
                    HttpContext.Logout_Workaround();
                    //redirects to the returnUrl with an error
                    return Redirect("/oops/facebook".QueryString("error", "User has been Blocked"));
                }

                //user doesn't exist in the database, must create a new instance
                if (user == null) user = new OrigamiSocialProfile { SocialNetwork = SocialNetworks.Facebook, UserId = userId };

                if (me != null)
                {
                    user.EmailFromSocialNetwork = me.email;
                    user.FirstName = me.first_name;
                    user.LastName = me.last_name;
                }

                if (pc?.data?.url != null) user.ProfilePictureUrl = pc.data.url;
                if (me?.link != null) user.ProfilePage = me.link;
                if (me?.cover != null && me?.cover.source != null) user.ProfileCoverUrl = me!.cover.source;
                if (cover != null && cover!.source != null) user.ProfileCoverUrl = cover!.source;

                var context = new DataOperationContext<OrigamiSocialProfile>(OrigamiUser.AnonymousUser, DateTime.UtcNow, user);

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
            return Redirect("/oops/facebook".QueryString("error", "Invalid Facebook Token"));
        }

        /// <summary>
        /// Verifies whether the Access Token is OK or not.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet("token-ok")]
        public bool TokenOk([FromQuery] string userId, [FromQuery] string accessToken)
        {
            if (userId.Has() == false) return false;
            if (accessToken.Has() == false) return false;

            try
            {
                //facebook client
                var FB = new FacebookClient
                {
                    AppId = _socialNetwork.Facebook.AppId,
                    AppSecret = _socialNetwork.Facebook.AppSecret,
                };

                var appToken = AppToken;

                dynamic r = FB.Get("/debug_token", new
                {
                    input_token = accessToken,
                    access_token = appToken
                });

                dynamic? data = r?.data;

                if (data?.user_id == userId && data?.is_valid == true) return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Validating the Facebook Token: an exception just happened");
            }

            return false;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Json(new
            {
                url = $"{Request.Scheme}://{Request.Host}/socialprofiles/{Guid.NewGuid()}",
                confirmation_code = Guid.NewGuid().ToString(),
            });
        }
    }
}
