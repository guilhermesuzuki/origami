using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Transactions;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class MicrosoftController : Controller
    {
        protected readonly Serilog.ILogger _logger;
        protected readonly IUserFacade _userFacade;
        protected readonly IMemoryCache _memoryCache;
        protected readonly SocialNetwork _socialNetwork;
        protected readonly ISocialProfileRepository _socialProfile;
        protected readonly IConfiguration _configuration;

        protected readonly IEventRepository _eventRepository;

        public MicrosoftController(
            IEventRepository eventRepository,
            ISocialProfileRepository socialProfile,
            Serilog.ILogger logger,
            IUserFacade userFacade,
            IMemoryCache memoryCache,
            IOptions<SocialNetwork> socialNetworkOptions,
            IConfiguration configuration)
            : base()
        {
            _socialProfile = socialProfile;
            _logger = logger;
            _userFacade = userFacade;
            _memoryCache = memoryCache;
            _socialNetwork = socialNetworkOptions.Value;
            _configuration = configuration;
            _eventRepository = eventRepository;
        }

        [HttpGet("token-ok")]
        public async Task<JwtSecurityToken?> TokenOk(
            string userId,
            string token,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var issuer = $"https://login.microsoftonline.com/{_socialNetwork.Microsoft.TenantId}/v2.0";
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                issuer + "/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var discoveryDocument = await configurationManager.GetConfigurationAsync(ct);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                // Allow for some drift in server time
                // (a lower value is better; we recommend two minutes or less)
                ClockSkew = TimeSpan.FromMinutes(2),
                // See additional validation for aud below
                ValidateAudience = false,
            };

            try
            {
                var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var rawValidatedToken);
                var securityToken = (JwtSecurityToken)rawValidatedToken;

                if (securityToken.Claims.Any(x => x.Type == "oid" && x.Value == userId)) return securityToken;

                return null;
            }
            catch (SecurityTokenValidationException)
            {
                // Logging, etc.

                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet("get-user")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser([FromQuery] string userId, [FromQuery] string accessToken, [FromQuery] string accessTokenSecret, [FromQuery] string returnUrl)
        {
            _logger.Information("Parameters -> userId: {0}, accessToken: {1}", userId, accessToken);

            var ok = await TokenOk(userId, accessTokenSecret);
            if (ok != null)
            {
                //looks the user up in the database
                var user = _socialProfile
                    .ReadFromCache()
                    .FirstOrDefault(x => x.SocialNetwork == SocialNetworks.Microsoft && x.UserId == userId)
                    ?? new () { SocialNetwork = SocialNetworks.Microsoft, UserId = userId, IsBlocked = false, }
                    ;

                if (user.IsBlocked)
                {
                    //needs to log the user out, because the microsoft user couldn't be found
                    HttpContext.SignOutAsync().GetAwaiter().GetResult();
                    HttpContext.Logout_Workaround();
                    //redirects to the returnUrl with an error
                    return Redirect("/oops/microsoft".QueryString("error", "User has been blocked"));
                }

                user.EmailFromSocialNetwork = ok.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value ?? string.Empty;

                var http = new HttpClient();

                //required (access token in the header)
                http.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var me = await http.GetFromJsonAsync<MicrosoftUser>("https://graph.microsoft.com/v1.0/me");

                user.FirstName = me?.GivenName ?? string.Empty;
                user.LastName = me?.SurName ?? string.Empty;

                var http2 = new HttpClient();

                //required (access token in the header)
                http2.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                http2.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
                http2.DefaultRequestHeaders.Add("Accept", "*/*");
                http2.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                http2.DefaultRequestHeaders.Add("Client-Request-Id", Guid.NewGuid().ToString());
                http2.DefaultRequestHeaders.Add("Sdkversion", "GraphExplorer/4.0, graph-js/3.0.7 (featureUsage=6)");

                var photoResponse = await http2.GetAsync("https://graph.microsoft.com/v1.0/me/photo/");
                if (photoResponse.IsSuccessStatusCode)
                {
                    var content = await photoResponse.Content.ReadAsStringAsync();
                    var photo = JsonSerializer.Deserialize<MicrosoftUserPhoto>(content);

                    user.ProfilePictureUrl = photo != null && photo.OData_Context.Has() ? photo.OData_Context : null;
                }

                //default no-icon profile picture
                if (user.ProfilePictureUrl.Has() == false) user.ProfilePictureUrl = OrigamiConstants.NoUser;

                var context = new DataOperationContext<OrigamiSocialProfile>(OrigamiUser.AnonymousUser, DateTime.UtcNow, user);

                //saves the user into the database
                using (var transaction = new TransactionScope())
                {
                    var hub = _socialProfile.SmartSave(context, false);
                    if (hub.Ok == false)
                    {
                        //redirects to the returnUrl with an error
                        return Redirect("/oops/microsoft"
                            .QueryString("error", "Invalid microsoft information")
                            .QueryString("error_details", hub.Messages.Error())
                            );
                    }
                    transaction.Complete();
                    user = hub.Entity;
                }

                _userFacade.SocialProfile = user ?? new();
                _eventRepository.SocialProfileLogsIntoWebsite(context.Entity);
                
                return Redirect(Uri.UnescapeDataString(returnUrl));
            }

            //needs to log the user out, because the microsoft user couldn't be found
            HttpContext.SignOutAsync().GetAwaiter().GetResult();
            HttpContext.Logout_Workaround();

            //redirects to the returnUrl with an error
            return Redirect("/oops/microsoft".QueryString("error", "Invalid microsoft token"));
        }
    }
}
