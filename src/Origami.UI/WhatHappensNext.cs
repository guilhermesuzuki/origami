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
    public WhatHappensNext(NavigationManager navigationManager, ISuperRepository superRepository)
    {
        this.GhostOfTheNavigator = navigationManager;
        this.SuperRepository = superRepository;
        this.WhenClickingHere = WhenTheUserClicksHereInTheFrontEnd;
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
