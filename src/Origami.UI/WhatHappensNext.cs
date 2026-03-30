using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI;

public class WhatHappensNext : IWhatHappensNext
{
    protected readonly NavigationManager GhostOfTheNavigator;
    protected readonly ISuperRepository SuperRepository;

    /// <summary>
    /// Default constructor with DI
    /// </summary>
    /// <param name="navigationManager">used for navigation</param>
    public WhatHappensNext(NavigationManager navigationManager, IAppFacade appFacade, ISuperRepository superRepository)
    {
        this.GhostOfTheNavigator = navigationManager;
        this.SuperRepository = superRepository;

        this.WhenClickingHere = appFacade.Admin.GetValueOrDefault()
            ? this.WhenTheUserClicksHereInTheAdmin
            : this.WhenTheUserClicksHereInTheFrontEnd;
    }

    /// <summary>
    /// When the user clicks here, this event is triggered. It contains a default handler
    /// </summary>
    public event EventHandler<WhenIClickHereEventArgs> WhenClickingHere;

    public void WhenIClickHere(object? sender, WhenIClickHereEventArgs e)
    {
        if (e.StopPropagation)
        {
            WhenClickingHere.GetInvocationList().Last().DynamicInvoke(sender, e);
            return;
        }
        this.WhenClickingHere.Invoke(sender, e);
    }

    public void WhenTheUserClicksHereInTheAdmin(object? sender, WhenIClickHereEventArgs e)
    {
        var hyperlink = e.Entity switch
        {
            OrigamiCategory => $"/categories?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiPage => $"/pages?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiPost => $"/posts?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiSpecialMessage => $"/specialmessages?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiSpecialPage => $"/specialpages?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiVideo => $"/videos?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiSocialProfile => $"/socialprofiles?id={e.Entity.Id}",
            OrigamiQuickNote => $"/quicknotes?nano={(e.Entity as INanoId)?.NanoId}",
            OrigamiContentTag => $"/tags?slug={(e.Entity as ISlug)?.Slug}",
            _ => string.Empty,
        };

        if (e.Entity is OrigamiContentComment comment)
        {
            var content = this.SuperRepository.Contents.ReadFromCache().Id(comment.ContentId);
            hyperlink = content switch
            {
                OrigamiPost => $"/posts/comments?id={comment.Id}",
                OrigamiVideo => $"/videos/comments?id={comment.Id}",
                _ => string.Empty,
            };
        }

        if (hyperlink.Has() == true)
        {
            GhostOfTheNavigator.NavigateTo(hyperlink);
            return;
        }

        throw new InvalidOperationException("Navigation aborted: location could not be determined");
    }

    public void WhenTheUserClicksHereInTheFrontEnd(object? sender, WhenIClickHereEventArgs e)
    {
        // route by category
        if (e.Slug is OrigamiCategory category)
        {
            var blog = this.SuperRepository.Blogs.ReadFromCache().Id(category.BlogId);
            if (blog != null)
            {
                var hyperlink = blog.GetHyperlink(category, e.Entity as INanoId);
                GhostOfTheNavigator.NavigateTo($"{hyperlink}#content-start");
                return;
            }
            throw new InvalidOperationException("Navigation aborted: blog not found from the given category");
        }

        // route by tag
        if (e.Slug is OrigamiContentTag tag)
        {
            var blogs = from a in this.SuperRepository.Contents.ReadFromCache()
                        join b in this.SuperRepository.Blogs.ReadFromCache() on a.BlogId equals b.Id
                        where a.Id == tag.ContentId
                        select b;

            var blog = blogs.FirstOrDefault();
            if (blog != null)
            {
                var hyperlink = blog.GetHyperlink(tag, e.Entity as INanoId);
                GhostOfTheNavigator.NavigateTo($"{hyperlink}#content-start");
                return;
            }
            throw new InvalidOperationException("Navigation aborted: blog not found from the given tag");
        }

        // default route by hyperlink
        if (e.Entity is IHyperlink link)
        {
            GhostOfTheNavigator.NavigateTo($"{link.Hyperlink}#content-start");
            return;
        }

        throw new InvalidOperationException("Navigation aborted: unable to determine navigation target");
    }
}
