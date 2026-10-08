using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.UI.Components
{
    public sealed class RazorComponentRenderer(HtmlRenderer renderer)
    {
        public Task<string> RenderAsync<TComponent>(
            IDictionary<string, object?> parameters)
            where TComponent : IComponent
        {
            var parameterView = ParameterView.FromDictionary(parameters);

            return renderer.Dispatcher.InvokeAsync(async () =>
            {
                var result = await renderer.RenderComponentAsync<TComponent>(parameterView).ConfigureAwait(true);
                return result.ToHtmlString();
            });
        }
    }
}
