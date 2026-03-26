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
    public abstract class BasicAdminGridContent<T1, T2> :
        BasicAdmin,
        ICreateEntity<T2>,
        IFilter
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        public string Filter { get; set; } = "all";

        [Inject] public IRepository<T1> Repository { get; set; } = null!;
        [Inject] public IHubContentRepository<T2> HubContentRepository { get; set; } = null!;

        /// <summary>
        /// DataGrid for this instance
        /// </summary>
        protected MudDataGrid<T1> DataGrid = null!;

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
        protected HashSet<T1> SelectedEntities { get; set; } = new();

        /// <summary>
        /// Selected entity
        /// </summary>
        protected T2 Root { get; set; } = new();

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
        public virtual void SelectedEntitiesChanged(HashSet<T1> newItems)
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
        protected virtual Result<T1> DeleteEntity(T1 entity)
        {
            var context = new DataOperationContext<T1>(this.UserFacade.User, DateTime.UtcNow, entity);
            return Repository.SmartDelete(context, true);
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
            Root = entity.Clone();
        }

        protected virtual void EntityCreated(T2? entity)
        {
            Root = entity.Clone();
            StateHasChanged();
        }

        /// <summary>
        /// Method to be called when the entity has been saved
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void EntitySaved(T2 entity)
        {
            DataGrid.ReloadServerData();
            Root = entity.Clone();
        }

        /// <summary>
        /// Executes method with <see cref="SelectedEntities"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="confirm">the MessageBox to confirm the execution of the method</param>
        /// <param name="mainStep">function to be executed in a transaction</param>
        /// <param name="additionalSteps">functions to be called after the transaction has been committed</param>
        /// <returns></returns>
        protected virtual async Task ExecuteWithSelectedEntities(Func<Task<bool?>> confirm, Func<T1, Result<T1>> mainStep, params Func<T1, Result<T1>>[] additionalSteps)
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
                var hub = new Result<T1>();
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
                    if (hub.Ok)
                    {
                        foreach (var step in additionalSteps)
                        {
                            step?.Invoke(entity).Push(hub);
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
            Root = this.HubContentRepository.Get(Root).Clone();
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<GridData<T1>> GetEntities(GridState<T1> state, CancellationToken token)
        {
            var orders = new StringBuilder();

            if (state.SortDefinitions.Any() == true)
            {
                //iterates through every sort definition
                foreach (var definition in state.SortDefinitions)
                {
                    orders.AppendFormat(",{0} {1}", definition.SortBy, definition.Descending ? "DESC" : "ASC");
                }
            }
            else
            {
                orders.Append($",{DefaultOrdering}", DefaultOrdering.Has());
            }

            var filters = new StringBuilder("(true)");

            //needs to filter by all, published or drafts
            if (this is IFilter filter)
            {
                if (filter.Filter.Like("published") == true)
                {
                    if (typeof(T1).Implements<IPublished>() == true)
                    {
                        filters.Append($" && ({nameof(IPublished.IsPublished)} == true)");
                    }
                }
                else if (filter.Filter.Like("drafts") == true)
                {
                    if (typeof(T1).Implements<IDraft>() == true)
                    {
                        filters.Append($" && ({nameof(IDraft.IsDraft)} == true)");
                    }
                }
            }

            //pulls information from cache
            var result = GetItems().Query(
                state.PageSize,
                state.Page * state.PageSize,
                filters.ToString(),
                orders.ToString()[1..]
            );

            return Task.FromResult(new GridData<T1> { Items = result.Rows, TotalItems = result.NumberOfRows, });
        }

        /// <summary>
        /// Retrieves all the items for the DataGrid.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<T1> GetItems()
        {
            IEnumerable<T1> items = Repository.ReadFromCache();

            if (IncludeDeletedEntitiesInDataGrid == false)
            {
                items = items.NonDeleted();
            }

            if (typeof(T1).Implements<IBlogId>() == true)
            {
                var query = from a in items.Cast<IBlogId>()
                            where a.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T1>();
            }

            if (typeof(T1).Implements<IBlogIdNull>() == true)
            {
                var query = from a in items.Cast<IBlogIdNull>()
                            where a.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T1>();
            }

            if (typeof(T1).Implements<IContentId>() == true)
            {
                var query = from a in items.Cast<IContentId>()
                            join b in Repository.ReadFromCache<OrigamiContent>() on a.ContentId equals b.Id
                            where b.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T1>();
            }

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
        protected virtual void OnEdit(T1 entity)
        {
            Root = this.HubContentRepository.Get(entity).Clone();
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
        }

        protected virtual void OnSearchResultSelected(T1 entity)
        {
            Root = this.HubContentRepository.Get(entity).Clone();
        }

        /// <summary>
        /// Purges the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T1> PurgeEntity(T1 entity)
        {
            var context = new DataOperationContext<T1>(this.UserFacade.User, DateTime.UtcNow, entity);
            return Repository.SmartPurge(context, true);
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
            Root = CreateEntity();
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
            Root = Root.Id == entity.Id ? CreateEntity() : Root;
            return new(entity);
        }

        /// <summary>
        /// Restores the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T1> RestoreEntity(T1 entity)
        {
            var context = new DataOperationContext<T1>(this.UserFacade.User, DateTime.UtcNow, entity);
            return Repository.SmartRestore(context, true);
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

        protected virtual string RowClassFunc(T1 entity, int index)
        {
            var result = string.Empty;
            if (entity.Id == Root.Id) result += " selected-row";
            if (entity is IPublished published1 && published1.IsPublished == false) result += " unpublished";
            if (entity is IPublished published2 && published2.IsPublished == true) result += " published";
            if (entity is IDeleted deleted && deleted.IsDeleted == true) result += " deleted";
            if (entity is OrigamiPage page && page.IsFrontPage == true) result += " front-page";
            if (entity is OrigamiCategory category && this.Super.IsParentDeleted(category) == true) result += " deleted";
            if (entity is OrigamiPage page2 && this.Super.IsParentDeleted(page2) == true) result += " deleted";
            return result;
        }

        /// <summary>
        /// Sometimes you need the <see cref="SelectedEntities"/> to be in a different order.
        /// </summary>
        /// <returns></returns>
        protected virtual List<T1> SelectedEntitiesInOrder()
        {
            return SelectedEntities.ToList();
        }
    }
}
