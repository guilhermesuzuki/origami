using Microsoft.JSInterop;
using Origami.Core.Models;

namespace Origami.UI.Admin
{
    public abstract class BasicAdmin : BasicPage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        protected BasicAdmin() : base()
        {

        }

        protected virtual Result CanAccess()
        {
            return new();
        }

        protected virtual async Task LogoutFromAdminAsync()
        {
            await this.JSRuntime.InvokeVoidAsync("$.removeCookie", this.Configuration.GetUserCookieKey(), new { path = "/" });
            this.UserFacade.User = OrigamiUser.AnonymousUser;
            this.GhostOfTheNavigator.Refresh(true);
        }

        protected override async Task PageTitleAsync(bool firstRender)
        {
            var title = $"{Text.Lower("Admin")}: {PageTitle.GetTitle()}";
            await JSRuntime.InvokeVoidAsync("origami.common.title", title);
        }

        protected override async Task PageViewAsync(bool firstRender)
        {
            if (firstRender)
            {
                var uri = new Uri(GhostOfTheNavigator.Uri);
                await JSRuntime.InvokeVoidAsync("origami.physicalpages.viewByPath", uri.AbsolutePath);
            }
        }
    }
}
