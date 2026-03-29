using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Origami.Core.Data;
using Origami.Core.Models;

namespace Origami.UI;

public class WhatHappensNext : IWhatHappensNext
{
    protected readonly NavigationManager GhostOfTheNavigator;

    /// <summary>
    /// Default constructor with DI
    /// </summary>
    /// <param name="navigationManager">used for navigation</param>
    public WhatHappensNext(NavigationManager navigationManager)
    {
        this.GhostOfTheNavigator = navigationManager;
        this.WhenClickingHere = WhenTheUserClicksHere;
    }

    /// <summary>
    /// When the user clicks here, this event is triggered. It contains a default handler
    /// </summary>
    public event EventHandler<WhenIClickHereEventArgs> WhenClickingHere;

    public void WhenIClickHere(object? sender, WhenIClickHereEventArgs e)
    {
        if (WhenClickingHere.GetInvocationList().Any() == true)
        {
            if (e.StopPropagation)
            {
                WhenClickingHere.GetInvocationList().LastOrDefault()?.DynamicInvoke(sender, e);
                return;
            }
            this.WhenClickingHere.Invoke(sender, e);
        }
    }

    public void WhenTheUserClicksHere(object? sender, WhenIClickHereEventArgs e)
    {
        if (e.Entity is IHyperlink hyperlink)
        {
            // Navigate to the details page for the entity
            GhostOfTheNavigator.NavigateTo(hyperlink.Hyperlink);
        }
    }
}
