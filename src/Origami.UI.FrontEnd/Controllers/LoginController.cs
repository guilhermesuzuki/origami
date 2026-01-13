using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Security.Claims;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class LoginController : Controller
    {
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Text _text;
        private readonly IUserFacade _userFacade;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserStore<IdentityUser> _userStore;
        public LoginController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            IUserFacade userFacade,
            Text text,
            IUserRepository userRepository)
            : base()
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _userFacade = userFacade;
            _text = text;

            _userRepository = userRepository;
        }

        [HttpGet("{provider}")]
        public IActionResult Login([FromRoute] string provider, [FromQuery] string? returnUrl = null)
        {
            //Google, Twitter or Facebook
            provider = provider[0].ToString().ToUpper() + provider[1..].ToLower();

            //fix for GitHub
            //fix for OpenIdConnect
            provider = provider switch
            {
                "Github" => "GitHub",
                "Openidconnect" => "OpenIdConnect",
                _ => provider,
            };

            var redirectUrl = $"/login/callback?returnUrl={Uri.EscapeDataString(returnUrl ?? string.Empty)}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("/");
            if (remoteError == null)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info != null)
                {
                    // Sign in the user with this external login provider if the user already has a login.
                    var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

                    var id = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                    //microsoft
                    if (info.ProviderDisplayName.Like("Microsoft") == true)
                    {
                        id = info.Principal.FindFirstValue("oid") ?? info.Principal.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                        email = info.Principal.FindFirstValue("preferred_username");
                    }

                    if (id == null)
                    {
                        var error = _text.Lower(Text.SomethingWentWrongPleaseTryAgain);
                        return Redirect(returnUrl.QueryString("error", error));
                    }

                    var user = await _userManager.FindByNameAsync(id);
                    if (user == null)
                    {
                        //must create the user
                        user = CreateUser();

                        await _userStore.SetUserNameAsync(user, id, CancellationToken.None);
                        await _emailStore.SetEmailAsync(user, email, CancellationToken.None);

                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded == false)
                        {
                            var error = result.Errors.Error();
                            return Redirect(returnUrl.QueryString("error", error));
                        }
                    }

                    var login = (await _userManager.GetLoginsAsync(user)).FirstOrDefault(x => x.LoginProvider == info.LoginProvider && x.ProviderKey == info.ProviderKey);
                    if (login == null)
                    {
                        var result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded == false)
                        {
                            var error = result.Errors.Error();
                            return Redirect(returnUrl.QueryString("error", error));
                        }
                    }

                    //signs the user in, using the manager
                    await _signInManager.SignInAsync(user, isPersistent: true, info.LoginProvider);

                    // code dealing with the request
                    var accessToken = info.AuthenticationTokens?.FirstOrDefault(x => x.Name == "access_token")?.Value;
                    var accessTokenSecret = info.AuthenticationTokens?.FirstOrDefault(x => x.Name == "access_token_secret")?.Value;
                    var name = info.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                    //microsoft
                    if (info.ProviderDisplayName.Like("Microsoft") == true)
                    {
                        name = info.Principal.FindFirstValue("name");
                        accessTokenSecret = info.AuthenticationTokens?.FirstOrDefault(x => x.Name == "id_token")?.Value;
                    }

                    //redirects to the get-user method of the appropriate controller
                    return Redirect($"/{info.ProviderDisplayName?.ToLower()}/get-user?userId={user.UserName}&name={name}&accessToken={accessToken}&accessTokenSecret={accessTokenSecret}&returnUrl=" + Uri.EscapeDataString(returnUrl));
                }
            }

            return Redirect(returnUrl.QueryString("error", _text.Lower(Text.SomethingWentWrongPleaseTryAgain)));
        }

        [HttpGet("out")]
        public async Task<IActionResult> Out([FromQuery] string returnUrl)
        {
            await HttpContext.SignOutAsync();
            HttpContext.Logout_Workaround();
            _userFacade.SocialProfile = new();
            return Redirect(Uri.UnescapeDataString(returnUrl));
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }

            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
