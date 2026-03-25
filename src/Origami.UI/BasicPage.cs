using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Origami.Core;
using Origami.Core.Data;

namespace Origami.UI
{
    public class BasicPage : Basic
    {
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
            var error = this.NavigationManager.Uri.QueryString(key);
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
            var language = this.NavigationManager.Uri.QueryString(key);
            if (language.Has() == true)
            {
                await JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", key);

                var hub = await SetLanguage(language);
                if (hub.Ok == false)
                {
                    this.UserFacade.Result = hub;
                    return;
                }

                this.NavigationManager.Refresh(true);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            await PageAsync(firstRender);
            await PageViewAsync(firstRender);
            await PageTitleAsync(firstRender);
            await ErrorFromQueryStringAsync();
            await LanguageFromQueryStringAsync();
        }

        protected virtual async Task PageAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("origami.common.lazy");
                await JSRuntime.InvokeVoidAsync("origami.common.yoxview");
                await JSRuntime.InvokeVoidAsync("origami.common.prism");
            }
            ChangeBlog();
        }

        protected virtual async Task PageTitleAsync(bool firstRender)
        {
            this.SetPageTitle();
            var title = PageTitle.GetTitle();
            await JSRuntime.InvokeVoidAsync("origami.common.title", title);
        }

        protected virtual async Task PageViewAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (this.UserFacade.IncognitoMode == false)
                {
                    var uri = new Uri(NavigationManager.Uri);
                    await JSRuntime.InvokeVoidAsync("origami.physicalpages.viewByPath", uri.AbsolutePath);
                }
            }
        }

        protected virtual void SetPageTitle()
        {
            this.PageTitle.SetTitle();
        }
    }
}
