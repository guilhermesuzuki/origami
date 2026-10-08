using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;
using UAParser;

namespace Origami.UI
{
    public class BasicPage : Basic
    {
        [Parameter] public bool ShouldSetPageTitle { get; set; } = true;
        [Parameter] public bool ShouldTrackUserVisit { get; set; } = true;
        [Inject] protected IPageTitleRepository PageTitle { get; set; } = null!;

        protected virtual void ChangeBlog()
        {
            if (BlogId == Guid.Empty) return;
            if (UserFacade.BlogId != BlogId)
            {
                UserFacade.BlogId = BlogId;
                UserFacade.Result = new() { Info = Text.Original("You switched to a different blog") };
            }
        }

        protected async Task ErrorFromQueryStringAsync()
        {
            var key = "error";
            var error = this.GhostOfTheNavigator.Uri.QueryString(key);
            if (error.Has() == true)
            {
                await JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", key);
                UserFacade.Result = new()
                {
                    Id = new Guid("43CA37CD-5AA4-4EBF-9A37-5019E054704F"),
                    Error = error,
                };
            }
        }

        protected async Task LanguageFromQueryStringAsync()
        {
            var key = "language";
            var language = this.GhostOfTheNavigator.Uri.QueryString(key);
            if (language.Has() == true)
            {
                await JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", key);

                var hub = await SetLanguage(language);
                if (hub.Ok == false)
                {
                    this.UserFacade.Result = hub;
                    return;
                }

                this.GhostOfTheNavigator.Refresh(true);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await PageAsync(firstRender);
            await PageTitleAsync(firstRender);
            await PageViewAsync(firstRender);
            await ErrorFromQueryStringAsync();
            await LanguageFromQueryStringAsync();
        }

        protected virtual async Task PageAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("origami.common.lazy");
                await JSRuntime.InvokeVoidAsync("origami.common.prism");
            }
            ChangeBlog();
        }

        protected virtual async Task PageTitleAsync(bool firstRender)
        {
            if (this.ShouldSetPageTitle == false) return;
            this.SetPageTitle();
            var title = PageTitle.GetTitle();
            await JSRuntime.InvokeVoidAsync("origami.common.title", title);
        }

        protected virtual async Task PageViewAsync(bool firstRender)
        {
            if (firstRender == false) return;
            if (this.UserFacade.IncognitoMode == true) return;
            if (this.ShouldTrackUserVisit == false) return;
            await this.PhysicalPagesByPathAsync();
        }

        /// <summary>
        /// TODO: add texts to RESX files
        /// </summary>
        /// <param name="contentId"></param>
        /// <returns></returns>
        protected async Task PhysicalPagesByContentAsync(Guid contentId)
        {
            var path = new Uri(this.GhostOfTheNavigator.Uri).AbsolutePath;

            await this.JSRuntime.InvokeAsync<RequestContext>(
                "origami.physicalpages.viewByContent",
                path, 
                contentId, 
                this.UserFacade.UserId, 
                this.UserFacade.SocialProfileId);

            this.UserFacade.RefreshTheUI(OrigamiConstants.Events.UpdateCounters);
        }

        /// <summary>
        /// TODO: add texts to RESX files
        /// </summary>
        /// <returns></returns>
        protected async Task PhysicalPagesByPathAsync()
        {
            var path = new Uri(this.GhostOfTheNavigator.Uri).AbsolutePath;

            await this.JSRuntime.InvokeAsync<RequestContext>(
                "origami.physicalpages.viewByPath",
                path,
                this.UserFacade.UserId,
                this.UserFacade.SocialProfileId);

            this.UserFacade.RefreshTheUI(OrigamiConstants.Events.UpdateCounters);
        }

        protected virtual void SetPageTitle()
        {
            this.PageTitle.SetTitle();
        }
    }
}
