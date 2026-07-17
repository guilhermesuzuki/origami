using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class GitHubController : Controller
    {
        protected readonly Serilog.ILogger _logger;
        protected readonly IUserFacade _userFacade;
        protected readonly IMemoryCache _memoryCache;
        protected readonly SocialNetwork _socialNetwork;
        protected readonly ISocialProfileRepository _socialProfile;

        protected readonly IEventRepository _eventRepository;

        public GitHubController(
            IEventRepository eventRepository,
            ISocialProfileRepository socialProfile,
            Serilog.ILogger logger,
            IUserFacade userFacade,
            IMemoryCache memoryCache,
            IOptions<SocialNetwork> socialNetworkOptions) : base()
        {
            _socialProfile = socialProfile;
            _logger = logger;
            _userFacade = userFacade;
            _memoryCache = memoryCache;
            _socialNetwork = socialNetworkOptions.Value;
            _eventRepository = eventRepository;
        }

        [AllowAnonymous]
        [HttpGet("get-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser([FromQuery] string userId, [FromQuery] string name, [FromQuery] string accessToken, [FromQuery] string returnUrl)
        {
            var client = new GitHubClient(new ProductHeaderValue(_socialNetwork.GitHub.AppName))
            {
                Credentials = new Credentials(accessToken)
            };

            var git = await client.User.Get(name);
            if (git != null)
            {
                //looks the user up in the database
                var user = _socialProfile
                    .ReadFromCache()
                    .FirstOrDefault(x => x.SocialNetwork == SocialNetworks.GitHub && x.UserId.Like(userId));

                if (user != null && user.IsBlocked)
                {
                    //needs to log the user out, because the facebook user couldn't be found
                    await HttpContext.SignOutAsync();
                    HttpContext.Logout_Workaround();

                    //redirects to the returnUrl with an error
                    return Redirect("/oops/github".QueryString("error", "User has been Blocked"));
                }

                //user doesn't exist in the database, must create a new instance
                if (user == null) user = new OrigamiSocialProfile { SocialNetwork = SocialNetworks.GitHub, UserId = userId };

                //email
                user.EmailFromSocialNetwork = git.Email;
                user.Name = git.Name;
                user.ProfilePage = git.HtmlUrl;
                user.ProfilePictureUrl = git.AvatarUrl;

                //copies the email, if appropriate
                if (user.Email.Has() == false && user.EmailFromSocialNetwork.Has() == true)
                {
                    user.Email = user.EmailFromSocialNetwork;
                }

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
            await HttpContext.SignOutAsync();
            HttpContext.Logout_Workaround();

            //redirects to the returnUrl with an error
            return Redirect("/oops/github".QueryString("error", "Invalid GitHub Token"));
        }
    }
}
