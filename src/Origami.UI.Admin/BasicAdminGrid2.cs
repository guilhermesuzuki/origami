using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.ComponentModel;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin
{
    public abstract class BasicAdminGrid2<T> :
        BasicAdminGrid<T>
        where T : class, IId, new()
    {
        public bool IsSelectedEntityNotNull
        {
            get => this.SelectedEntity != null;
            set
            {
                if (value == false)
                {
                    this.SelectedEntity = null;
                    JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", "nanoid");
                }
            }
        }

        protected void New()
        {
            this.SelectedEntity = this.TheCreator.Create<T>();
        }

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
