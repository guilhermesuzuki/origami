using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Origami.UI
{
    public class BasicLayout : LayoutComponentBase
    {
        [Inject] protected IAppFacade AppFacade { get; set; } = null!;
        [Inject] protected AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject] protected IConfiguration Configuration { get; set; } = null!;
        [Inject] protected IDialogService DialogService { get; set; } = null!;
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = null!;
        [Inject] protected ISuperRepository Super { get; set; } = null!;
        [Inject] protected Text Text { get; set; } = null!;
        [Inject] protected IUserFacade UserFacade { get; set; } = null!;
        [Inject] protected IWebRootPath WebRootPath { get; set; } = null!;

        protected async Task CookieConsentAsync()
        {
            var cookie = this.HttpContextAccessor.HttpContext?.Request.Cookies["cookie-consent"] ?? string.Empty;
            this.UserFacade.ShowCookieConsent = new[] { string.Empty, "0", "false" }.Contains(cookie);
        }

        /// <summary>
        /// Loads the incognito mode from cookies
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected async Task LoadIncognitoModeAsync()
        {
            var cookie = this.HttpContextAccessor.HttpContext?.Request.Cookies["incognito-mode"] ?? string.Empty;
            this.UserFacade.IncognitoMode = new[] { "1", "true", }.Contains(cookie);
        }

        /// <summary>
        /// Loads the incognito mode from cookies
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected async Task LoadIncognitoModeAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.UserFacade.IncognitoMode = await JSRuntime.IncognitoModeAsync();
                this.StateHasChanged();
            }
        }

        protected virtual async Task LoadUserAsync()
        {
            var state = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();

            if (state == null) return;
            if (state.User.Identity == null) return;
            if (state.User.Identity.IsAuthenticated == false) return;

            var id = state.User.FindFirstValue(ClaimTypes.Name);
            if (id != null)
            {
                var method = state.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.AuthenticationMethod)?.Value;

                if (Enum.TryParse<SocialNetworks>(method, out var socialNetwork) == true)
                {
                    var socialProfile = this.Super.SocialProfiles.ReadFromCache()
                        .Where(x => x.SocialNetwork == socialNetwork)
                        .Where(x => x.UserId == id)
                        .FirstOrDefault();

                    this.UserFacade.SocialProfile = socialProfile ?? new();
                }
                else if (method.Like("OpenIdConnect") == true)
                {
                    var socialProfile = this.Super.SocialProfiles.ReadFromCache()
                        .Where(x => x.SocialNetwork == SocialNetworks.Microsoft)
                        .Where(x => x.UserId == id)
                        .FirstOrDefault();

                    this.UserFacade.SocialProfile = socialProfile ?? new();
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await this.LoadUserAsync();
            Task.WaitAll(this.LoadIncognitoModeAsync(), this.CookieConsentAsync());
            await base.OnInitializedAsync();
        }
    }
}
