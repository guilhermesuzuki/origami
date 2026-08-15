using Origami.Core;
using System.Security.Claims;

namespace Origami.UI
{
    public class BasicLayoutAdmin : BasicLayout
    {
        /// <summary>
        /// Loads the authenticated user
        /// </summary>
        /// <returns></returns>
        protected override async Task LoadUserAsync()
        {
            var state = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (state == null) return;
            if (state.User.Identity == null) return;
            if (state.User.Identity.IsAuthenticated == false) return;

            var nameId = state.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (nameId != null && Guid.TryParse(nameId, out var id) == true)
            {
                this.UserFacade.User = this.Super.Users.ReadFromCache().Id(id) ?? new();
            }
        }
    }
}
