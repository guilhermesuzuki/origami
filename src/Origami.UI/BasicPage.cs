using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
            var hub = await this.PhysicalPagesByPathAsync();
            if (hub.Ok == false)
            {
                this.UserFacade.Result = hub;
            }
        }

        protected async Task<Result> PhysicalPagesByContentAsync(Guid id)
        {
            var absolutePath = new Uri(this.GhostOfTheNavigator.Uri).AbsolutePath;
            if (absolutePath.Has() == false) absolutePath = "/";

            using var db = await this.DbContextFactory.CreateDbContextAsync();
            var pages = from p in db.Set<OrigamiPhysicalPage>().AsNoTracking() where p.Path.Equals(absolutePath) == true select p;

            var page = await pages.FirstOrDefaultAsync();
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = absolutePath,
                    DateCreated = this.Chronos.GetUtcNow().Date,
                };

                using (var transaction = new TransactionScope())
                {
                    var result = this.Super.PhysicalPages.SmartSave(page.GetContext(), false);
                    if (result.Ok == false)
                    {
                        return new() { Error = "Internal server error" };
                    }
                    transaction.Complete();
                }
            }

            if (page != null)
            {
                var view = new OrigamiPhysicalPageView
                {
                    Id = Guid.NewGuid(),
                    PhysicalPageId = page.Id,
                    Admin = this.AppFacade.Admin,
                    ContentId = id,
                    DateCreated = this.Chronos.GetUtcNow().Date,
                };
                var ok = this._fill(view);
                if (ok) 
                {
                    this.Super.PhysicalPageViews.SmartSave(view.GetContext(), false);
                    this.AppFacade.RefreshUI(this.HttpContextAccessor.HttpContext?.Connection.Id ?? string.Empty, OrigamiConstants.Events.UpdateCounters);
                    return new();
                }

                return new() { Error = "Metadata error (HttpContext null)" };
            }

            return new() { Error = "Page not found" };
        }

        protected async Task<Result> PhysicalPagesByPathAsync()
        {
            var absolutePath = new Uri(this.GhostOfTheNavigator.Uri).AbsolutePath;
            if (absolutePath.Has() == false) absolutePath = "/";

            using var db = await this.DbContextFactory.CreateDbContextAsync();

            var page = await db.Set<OrigamiPhysicalPage>().AsNoTracking().FirstOrDefaultAsync(x => x.Path.Equals(absolutePath) == true);
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = absolutePath,
                    DateCreated = Chronos.GetUtcNow().Date,
                };
                using (var transaction = new TransactionScope())
                {
                    var result = this.Super.PhysicalPages.SmartSave(page.GetContext(), false);
                    if (result.Ok == false)
                    {
                        return new() { Error = "Internal server error" };
                    }
                    transaction.Complete();
                }
            }
            if (page != null)
            {
                var view = new OrigamiPhysicalPageView
                {
                    Id = Guid.NewGuid(),
                    PhysicalPageId = page.Id,
                    Admin = this.AppFacade.Admin,
                    DateCreated = this.Chronos.GetUtcNow().Date,
                };
                var ok = this._fill(view);
                if (ok)
                {
                    this.Super.PhysicalPageViews.SmartSave(view.GetContext(), false);
                    this.AppFacade.RefreshUI(this.HttpContextAccessor.HttpContext?.Connection.Id ?? string.Empty, OrigamiConstants.Events.UpdateCounters);
                    return new();
                }

                return new() { Error = "Metadata error (HttpContext null)" };
            }

            return new() { Error = "Page not found" };
        }

        protected virtual void SetPageTitle()
        {
            this.PageTitle.SetTitle();
        }

        /// <summary>
        /// Fills the <paramref name="tracking"/> with request information
        /// </summary>
        /// <param name="tracking"></param>
        private bool _fill(BaseTracking tracking)
        {
            if (this.HttpContextAccessor.HttpContext == null)
            {
                return false;
            }

            var dd = this.HttpContextAccessor.HttpContext.Request.GetDeviceDetector();

            // important!
            dd.Parse();

            tracking.Url = this.GhostOfTheNavigator.Uri;
            tracking.UrlReferrer = this.HttpContextAccessor.HttpContext.Request.Headers.Referer.ToString();
            tracking.UserAgent = this.HttpContextAccessor.HttpContext.Request.Header("User-Agent");
            tracking.HostAddress = this.HttpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            tracking.IsMobileDevice = dd.IsTablet() || dd.IsMobile();
            tracking.IsBot = dd.IsBot();

            tracking.UserId = this.UserFacade.User.New == false ? this.UserFacade.User.Id : null;
            tracking.SocialProfileId = this.UserFacade.SocialProfile.New == false ? this.UserFacade.SocialProfile.Id : null;

            var client = Parser.GetDefault().Parse(tracking.UserAgent);

            tracking.Platform = client.OS.Family;
            tracking.Browser = client.UA.Family;

            var key = $"Origami_UserLocation_{this.HttpContextAccessor.HttpContext.Connection.Id}";
            tracking.Location = this.MemoryCache.Get<Location>(key);

            return true;
        }
    }
}
