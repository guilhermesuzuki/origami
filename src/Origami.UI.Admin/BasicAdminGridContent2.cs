using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin
{
    public abstract class BasicAdminGridContent2<T1, T2> :
        BasicAdminGridContent<T1, T2>
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        protected override async Task ReloadDataGridAsync()
        {
            SelectedEntity = null;
            SelectedEntities = new();
            await DataGrid.ReloadServerData();
            await this.InvokeAsync(this.StateHasChanged);
        }

        protected override void SetEntityFromQueryString()
        {
            base.SetEntityFromQueryString();

            if (this.NanoId.Has() == false)
            {
                this.SelectedEntity = null;
            }
        }
    }
}
