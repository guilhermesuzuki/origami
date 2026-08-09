using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Validators;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using SixLabors.ImageSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Transactions;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class MicrosoftController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected readonly IEventRepository _eventRepository;
        protected readonly HttpClient _httpClient;
        protected readonly Serilog.ILogger _logger;
        protected readonly IMemoryCache _memoryCache;
        protected readonly SocialNetwork _socialNetwork;
        protected readonly ISocialProfileRepository _socialProfile;
        protected readonly IUserFacade _userFacade;

        public MicrosoftController(
            IEventRepository eventRepository,
            ISocialProfileRepository socialProfile,
            Serilog.ILogger logger,
            IUserFacade userFacade,
            IMemoryCache memoryCache,
            IOptions<SocialNetwork> socialNetworkOptions,
            IConfiguration configuration,
            HttpClient httpClient)
            : base()
        {
            _configuration = configuration;
            _eventRepository = eventRepository;
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _socialNetwork = socialNetworkOptions.Value;
            _socialProfile = socialProfile;
            _userFacade = userFacade;
        }

        public async Task<byte[]?> GetProfilePhotoAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://graph.microsoft.com/v1.0/me/photo/$value");

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null; // User doesn't have a profile photo

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
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
                    ?? new() { SocialNetwork = SocialNetworks.Microsoft, UserId = userId, IsBlocked = false, }
                    ;

                if (user.IsBlocked)
                {
                    //needs to log the user out, because the microsoft user couldn't be found
                    await HttpContext.SignOutAsync();
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
                user.Name = me?.DisplayName ?? string.Empty;

                user.ProfileCover = null;
                user.ProfileCoverUrl = null;
                user.ProfilePage = null;
                user.ProfilePicture = null;
                user.ProfilePictureUrl = null;

                var photoBytes = await GetProfilePhotoAsync(accessToken);
                if (photoBytes != null)
                {
                    using var image = Image.Load(photoBytes);
                    user.ProfilePicture = image.ToBase64String(Image.DetectFormat(photoBytes));
                }

                var context = new DataOperationContext<OrigamiSocialProfile>(OrigamiUser.AnonymousUser, DateTime.UtcNow, user);

                //saves the user into the database
                using (var transaction = new TransactionScope())
                {
                    var hub = _socialProfile.SmartSave(context, false);
                    if (hub.Ok == false)
                    {
                        await HttpContext.SignOutAsync();
                        HttpContext.Logout_Workaround();
                        //redirects to the returnUrl with an error
                        return Redirect("/oops/microsoft"
                            .QueryString("error", "Invalid microsoft information")
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
            await HttpContext.SignOutAsync();
            HttpContext.Logout_Workaround();

            //redirects to the returnUrl with an error
            return Redirect("/oops/microsoft".QueryString("error", "Invalid microsoft token"));
        }

        [HttpGet("token-ok")]
        public async Task<JwtSecurityToken?> TokenOk(
            string userId,
            string token,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));

            var issuer = $"https://login.microsoftonline.com/common/v2.0";
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

                // IMPORTANT:
                // Do not set ValidIssuer to /common.
                IssuerValidator = AadIssuerValidator.GetAadIssuerValidator(issuer).Validate,

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
    }
}
