using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Charts;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin
{
    public abstract class BasicAdminGridContent<T1, T2> :
        BasicAdmin,
        ICreateEntity<T2>,
        IFilter
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        /// <summary>
        /// DataGrid for this instance
        /// </summary>
        protected MudDataGrid<T2> DataGrid = null!;

        public string Filter { get; set; } = "all";

        [Inject] public IHubContentRepository<T2> HubContentRepository { get; set; } = null!;
        [Inject] public IRepository<T1> Repository { get; set; } = null!;
        /// <summary>
        /// Default ordering, in case there's no order-by
        /// </summary>
        protected virtual string DefaultOrdering => string.Empty;

        /// <summary>
        /// Can the user delete the selected entities?
        /// </summary>
        protected virtual bool DisableTheDeleteButton => SelectedEntities.Any() == false;

        /// <summary>
        /// Can the user purge the selected entities?
        /// </summary>
        protected virtual bool DisableThePurgeButton => SelectedEntities.Any() == false;

        /// <summary>
        /// Can the user restore the selected entities?
        /// </summary>
        protected virtual bool DisableTheRestoreButton => SelectedEntities.Any() == false;

        /// <summary>
        /// Deleted entities should be listed in the DataGrid?
        /// </summary>
        protected virtual bool IncludeDeletedEntitiesInDataGrid { get; set; } = false;

        /// <summary>
        /// Selected entities
        /// </summary>
        protected HashSet<T2> SelectedEntities { get; set; } = new();

        /// <summary>
        /// Selected entity
        /// </summary>
        protected T2 SelectedEntity { get; set; } = new();

        public T2 CreateEntity()
        {
            var blog = this.GetBlogFromUserFacade();
            var root = new T2();
            root.Entity.SetId();
            root.Entity.SetBlog(blog);
            root.Entity.SetAuthor(this.UserFacade.User);
            return root;
        }

        /// <summary>
        /// Selected entities have changed (and need to be updated)
        /// </summary>
        /// <param name="newItems"></param>
        public virtual void SelectedEntitiesChanged(HashSet<T2> newItems)
        {
            SelectedEntities = newItems;
        }

        public virtual async Task SetFilterAndRefreshUI(string filter)
        {
            await JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", "filter");
            await JSRuntime.InvokeVoidAsync("addQueryStringWithoutReload", "filter", filter);
            await DataGrid.ReloadServerData();
            Filter = filter;
        }

        protected override Result CanAccess()
        {
            return HubContentRepository.CanRead(this.UserFacade.User);
        }

        /// <summary>
        /// Deletes the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T2> DeleteEntity(T2 entity)
        {
            return HubContentRepository.Delete(entity, this.UserFacade.User);
        }

        /// <summary>
        /// The user wants to delete certain entities
        /// </summary>
        /// <returns></returns>
        protected virtual async Task DeleteSelectedEntities()
        {
            await ExecuteWithSelectedEntities(
                async () =>
                {
                    return await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Deleting {0} Item(s)", SelectedEntities.Count),
                        Text.Original("Are you sure?"),
                        yesText: Text.Lower("Yes"),
                        noText: Text.Lower("No"));
                },
                this.DeleteEntity
            );
        }

        /// <summary>
        /// Method to be called when the entity's editing has been cancelled (<see cref="_BasicForm{T}.Cancelled"/>)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void EntityCancelled(T2? entity)
        {
            SelectedEntity = entity.Clone();
        }

        protected virtual void EntityCreated(T2? entity)
        {
            SelectedEntity = entity.Clone();
            StateHasChanged();
        }

        /// <summary>
        /// Method to be called when the entity has been saved
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void EntitySaved(T2 entity)
        {
            DataGrid.ReloadServerData();
            SelectedEntity = entity.Clone();
        }

        /// <summary>
        /// Executes method with <see cref="SelectedEntities"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="confirm">the MessageBox to confirm the execution of the method</param>
        /// <param name="mainStep">function to be executed in a transaction</param>
        /// <param name="additionalSteps">functions to be called after the transaction has been committed</param>
        /// <returns></returns>
        protected virtual async Task ExecuteWithSelectedEntities(Func<Task<bool?>> confirm, Func<T2, Result<T2>> mainStep)
        {
            var answer = await confirm();
            if (answer.GetValueOrDefault() == false) return;

            var entities = SelectedEntitiesInOrder();

            //checks to see if there's entities
            if (entities.Count == 0)
            {
                this.UserFacade.Result = new() { Info = Text.Original("No entities were selected") };
                return;
            }

            //iterates entities
            foreach (var entity in entities)
            {
                var hub = new Result<T2>();
                try
                {
                    using (var transaction = new TransactionScope())
                    {
                        hub = mainStep.Invoke(entity);
                        if (hub.Ok)
                        {
                            transaction.Complete();
                        }
                    }
                }
                catch (Exception ex)
                {
                    hub.Error = ex.GetMessage();
                }
                finally
                {
                    this.UserFacade.Result = hub;
                }
            }

            await ReloadDataGridAsync();
            SelectedEntity = this.HubContentRepository.Get(SelectedEntity.Entity).Clone();
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<GridData<T2>> GetEntities(GridState<T2> state, CancellationToken token)
        {
            var orders = new StringBuilder($",{DefaultOrdering}");
            var filters = new StringBuilder("(true)");

            // pulls information from cache
            var result = GetItems().Query(
                state.PageSize,
                state.Page * state.PageSize,
                filters.ToString(),
                orders.ToString()[1..]
            );

            var data = new GridData<T2>();

            data.Items = result.Rows.Select(x => this.HubContentRepository.Get(x)).ToList();
            data.TotalItems = result.NumberOfRows;

            return Task.FromResult(data);
        }

        /// <summary>
        /// Retrieves all the items for the DataGrid.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<T1> GetItems()
        {
            var t1 = new T1();
            using var db = this.DbContextFactory.CreateDbContext();

            IEnumerable<T1> items = db.ReadFromCache<T1>(this.MemoryCache);

            if (IncludeDeletedEntitiesInDataGrid == false)
            {
                items = items.NonDeleted();
            }

            if (t1 is IBlogId)
            {
                var query = from a in items.Cast<IBlogId>()
                            where a.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T1>();
            }

            if (t1 is IBlogIdNull)
            {
                var query = from a in items.Cast<IBlogIdNull>()
                            where a.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T1>();
            }

            items = this.Filter switch
            {
                "Drafts" => from item in items where item.IsDraft == true select item,
                "Published" => from item in items where item.IsPublished == true select item,
                _ => items
            };

            return items;
        }

        /// <summary>
        /// Has the Blog property changed in UserFacade?
        /// </summary>
        protected virtual void HasBlogChangedInUserFacade()
        {
            this.UserFacade.Changed += async (sender, p) =>
            {
                if (p.PropertyName.Like(nameof(IUserFacade.BlogId)) == true)
                {
                    await ReloadDataGridAsync();
                    await this.InvokeAsync(this.StateHasChanged);
                }
            };
        }

        /// <summary>
        /// User selects an <paramref name="entity"/> to edit
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnEdit(T2 entity)
        {
            SelectedEntity = entity.Clone();
        }

        /// <summary>
        /// User wants to list or hide deleted entities, so it should reload the datagrid.
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        protected virtual async Task OnIncludeDeletedEntitiesChanged(bool newValue)
        {
            IncludeDeletedEntitiesInDataGrid = newValue;
            await ReloadDataGridAsync();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            HasBlogChangedInUserFacade();

            var filter = this.NavigationManager!.Uri.QueryString("filter");
            this.Filter = filter.Has() ? filter : this.Filter;
        }

        protected virtual void OnSearchResultSelected(T1 entity)
        {
            SelectedEntity = this.HubContentRepository.Get(entity).Clone();
        }

        protected virtual Result<T2> PublishEntity(T2 entity)
        {
            return HubContentRepository.Publish(entity, this.UserFacade.User);
        }

        protected async Task PublishSelectedEntities()
        {
            await ExecuteWithSelectedEntities(
                async () =>
                {
                    return await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Publish {0} item(s)", SelectedEntities.Count),
                        Text.Original("Are you sure?"),
                        yesText: Text.Lower("Yes"),
                        noText: Text.Lower("No"));
                },
                this.PublishEntity
            );
        }

        /// <summary>
        /// Purges the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T2> PurgeEntity(T2 entity)
        {
            return HubContentRepository.Purge(entity, this.UserFacade.User);
        }

        /// <summary>
        /// The user wants to purge certain entities
        /// </summary>
        /// <returns></returns>
        protected virtual async Task PurgeSelectedEntities()
        {
            await ExecuteWithSelectedEntities(
                async () =>
                {
                    return await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Purging {0} Item(s)", SelectedEntities.Count),
                        Text.Original("Are you sure? You will NOT be able to recover these items."),
                        yesText: Text.Lower("Yes"),
                        noText: Text.Lower("No"));
                },
                this.PurgeEntity
            );
        }

        /// <summary>
        /// Reloads the data-grid
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ReloadDataGridAsync()
        {
            SelectedEntity = CreateEntity();
            SelectedEntities = new();
            await DataGrid.ReloadServerData();
        }

        /// <summary>
        /// Clean <paramref name="entity"/> from cache and its children
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Result<T1> RemoveEntityFromCache(T1 entity)
        {
            Repository.PurgeCache(entity);
            SelectedEntity = SelectedEntity.Id == entity.Id ? CreateEntity() : SelectedEntity;
            return new(entity);
        }

        /// <summary>
        /// Restores the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T2> RestoreEntity(T2 entity)
        {
            return HubContentRepository.Restore(entity, this.UserFacade.User);
        }

        /// <summary>
        /// The user wants to purge certain entities
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RestoreSelectedEntities()
        {
            await ExecuteWithSelectedEntities(
                async () =>
                {
                    return await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Restoring {0} Item(s)", SelectedEntities.Count),
                        Text.Original("Are you sure?"),
                        yesText: Text.Lower("Yes"),
                        noText: Text.Lower("No"));
                },
                this.RestoreEntity
            );
        }

        protected virtual string RowClassFunc(T2 entity, int index)
        {
            var result = string.Empty;
            if (entity.Id == SelectedEntity.Id) result += " selected-row";
            if (entity.Entity is IPublished published1 && published1.IsPublished == false) result += " unpublished";
            if (entity.Entity is IPublished published2 && published2.IsPublished == true) result += " published";
            if (entity.Entity is IDeleted deleted && deleted.IsDeleted == true) result += " deleted";
            if (entity.Entity is OrigamiPage page && page.IsFrontPage == true) result += " front-page";
            if (entity.Entity is OrigamiCategory category && this.Super.IsParentDeleted(category) == true) result += " deleted";
            if (entity.Entity is OrigamiPage page2 && this.Super.IsParentDeleted(page2) == true) result += " deleted";
            return result;
        }

        /// <summary>
        /// Sometimes you need the <see cref="SelectedEntities"/> to be in a different order.
        /// </summary>
        /// <returns></returns>
        protected virtual List<T2> SelectedEntitiesInOrder()
        {
            return SelectedEntities.ToList();
        }
        protected virtual Result<T2> UnpublishEntity(T2 entity)
        {
            return HubContentRepository.Unpublish(entity, this.UserFacade.User);
        }
        protected async Task UnpublishSelectedEntities()
        {
            await ExecuteWithSelectedEntities(
                async () =>
                {
                    return await DialogService.ShowMessageBoxAsync(
                        Text.Upper("Unpublish {0} item(s)", SelectedEntities.Count),
                        Text.Original("Are you sure?"),
                        yesText: Text.Lower("Yes"),
                        noText: Text.Lower("No"));
                },
                this.UnpublishEntity
            );
        }
    }
}
