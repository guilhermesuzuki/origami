using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using Origami.Core.Models.FileSystem;
using System.Globalization;

namespace Origami.UI
{
    public abstract class Basic :
        ComponentBase,
        IClass,
        IId,
        IBlogId
    {
        /// <summary>
        /// Sync root object
        /// </summary>
        public static readonly Lock SyncRoot = new();

        [Parameter] public Guid BlogId { get; set; }
        [Parameter] public string BlogSlug { get; set; } = string.Empty;
        [Parameter] public virtual string Class { get; set; } = string.Empty;

        /// <summary>
        /// Identifier for this instance.
        /// </summary>
        [Parameter] public virtual Guid Id { get; set; } = Guid.Empty;

        [Inject] protected IAppFacade AppFacade { get; set; } = null!;
        [Inject] protected IConfiguration Configuration { get; set; } = null!;
        [Inject] protected IDbContextFactory<OrigamiDbContext> DbContextFactory { get; set; } = null!;
        [Inject] protected IDialogService DialogService { get; set; } = null!;
        [Inject] protected NavigationManager GhostOfTheNavigator { get; set; } = null!;
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] protected IMyMemoryCache MemoryCache { get; set; } = null!;
        [Inject] protected ISuperRepository Super { get; set; } = null!;
        [Inject] protected Text Text { get; set; } = null!;
        [Inject] protected IUserFacade UserFacade { get; set; } = null!;
        [Inject] protected IWebRootPath WebRootPath { get; set; } = null!;
        [Inject] protected IWhatHappensNext WhatHappensNext { get; set; } = null!;

        public OrigamiBlog GetBlogFromSlug()
        {
            if (this.BlogSlug.Has() == true)
            {
                return Super.Blogs.ReadFromCache().Slug(BlogSlug) ?? OrigamiBlog.Empty;
            }

            return Super.Blogs.ReadFromCache().Single(x => x.IsPrimary);
        }

        public OrigamiBlog GetBlogFromUserFacade()
        {
            return Super.Blogs.ReadFromCache().Id(UserFacade.BlogId) ?? OrigamiBlog.Empty;
        }

        protected async Task CopyToClipboard(string info)
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", info);
        }

        protected async Task DownloadFile(OrigamiSystemFile file)
        {
            await this.JSRuntime.InvokeVoidAsync("origami.common.downloadFileFromUrl", file.WebPath);
        }

        /// <summary>
        /// For self-calling the application
        /// </summary>
        /// <returns></returns>
        protected virtual HttpClient GetHttpClient()
        {
            var baseUri = this.HttpContextAccessor.HttpContext?.Request.Scheme + "://" +
                          this.HttpContextAccessor.HttpContext?.Request.Host.Value;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUri),
                DefaultRequestVersion = new Version(2, 0),
                Timeout = TimeSpan.FromSeconds(30)
            };

            return client;
        }

        /// <summary>
        /// Logs the SOCIAL PROFILE out and redirects to the login page.
        /// </summary>
        protected virtual void Logout()
        {
            var returnUrl = Uri.EscapeDataString(GhostOfTheNavigator.Uri);
            GhostOfTheNavigator!.NavigateTo($"/login/out?returnUrl={returnUrl}", true);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.UserFacade.Changed += _currentBlogIdChangedMustRefreshUI;
        }

        /// <summary>
        /// Sets the new language
        /// </summary>
        /// <param name="language">en-US, pt-BR, etc.</param>
        /// <returns></returns>
        protected virtual async Task<Result> SetLanguage(string language)
        {
            try
            {
                var ui = new CultureInfo(language);

                var key = CookieRequestCultureProvider.DefaultCookieName;
                var value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(ui, ui));

                Thread.CurrentThread.CurrentCulture = ui;
                Thread.CurrentThread.CurrentUICulture = ui;

                await JSRuntime.InvokeVoidAsync("$.cookie", key, value, new { path = "/", expires = 1 * 180 });
                return new() { Id = new Guid("930643FD-1EAA-4516-A00F-76E5DDC84610") };
            }
            catch (Exception ex)
            {
                return new(ex) { Id = new Guid("B8842757-1509-41DB-863C-BDC8A6D23DEB") };
            }
        }

        /// <summary>
        /// Subscribes the user into the website
        /// </summary>
        protected void Subscribe()
        {
            if (UserFacade.SocialProfile.HasEmail() == true)
            {
                UserFacade.Result = Super.Subscribers.Subscribe(new(OrigamiUser.AnonymousUser, DateTime.UtcNow, UserFacade.SocialProfile), UserFacade.SocialProfile.EmailFromSocialNetwork);
                return;
            }
            GhostOfTheNavigator.NavigateTo($"/subscribe", false);
        }

        /// <summary>
        /// Unsubscribes the user from the website
        /// </summary>
        protected virtual void Unsubscribe()
        {
            var ctx = this.UserFacade.SocialProfile.GetContext();
            UserFacade.Result = Super.Subscribers.Unsubscribe(ctx);
        }

        private void _currentBlogIdChangedMustRefreshUI(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IUserFacade.BlogId))
            {
                this.InvokeAsync(this.StateHasChanged);
            }
        }
    }
}
