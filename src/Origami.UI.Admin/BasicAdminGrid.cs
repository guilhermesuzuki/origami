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
    public abstract class BasicAdminGrid<T> :
        BasicAdmin,
        ICreateEntity<T>,
        IFilter
        where T : class, IId, new()
    {
        public string Filter { get; set; } = "all";
        [Inject] public IRepository<T> Repository { get; set; } = null!;

        /// <summary>
        /// DataGrid for this instance
        /// </summary>
        protected MudDataGrid<T>? DataGrid { get; set; }

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
        protected HashSet<T> SelectedEntities { get; set; } = new();

        /// <summary>
        /// Selected entity
        /// </summary>
        protected T SelectedEntity { get; set; } = new();

        public T CreateEntity()
        {
            var blog = this.GetBlogFromUserFacade();
            var entity = new T();
            entity.SetId();
            entity.SetBlog(blog);
            entity.SetAuthor(this.UserFacade.User);
            return entity;
        }

        /// <summary>
        /// Selected entities have changed (and need to be updated)
        /// </summary>
        /// <param name="newItems"></param>
        public virtual void SelectedEntitiesChanged(HashSet<T> newItems)
        {
            SelectedEntities = newItems;
        }

        public virtual async Task SetFilterAndRefreshUI(string filter)
        {
            await JSRuntime.InvokeVoidAsync("removeQueryStringWithoutReload", "filter");
            await JSRuntime.InvokeVoidAsync("addQueryStringWithoutReload", "filter", filter);
            Filter = filter;
            DataGrid?.ReloadServerData();
        }

        protected override Result CanAccess()
        {
            return Repository.CanRead(this.UserFacade.User.Id);
        }

        /// <summary>
        /// Deletes the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T> DeleteEntity(T entity)
        {
            var context = new DataOperationContext<T>(this.UserFacade.User, DateTime.UtcNow, entity);
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
        protected virtual void EntityCancelled(T? entity)
        {
            SelectedEntity = entity.Clone();
        }

        protected virtual void EntityCreated(T? entity)
        {
            SelectedEntity = entity.Clone();
            StateHasChanged();
        }

        /// <summary>
        /// Method to be called when the entity has been saved
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void EntitySaved(T entity)
        {
            DataGrid?.ReloadServerData();
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
        protected virtual async Task ExecuteWithSelectedEntities(Func<Task<bool?>> confirm, Func<T, Result<T>> mainStep, params Func<T, Result<T>>[] additionalSteps)
        {
            var answer = await confirm();
            if (answer.GetValueOrDefault() == false) return;

            var entities = SelectedEntitiesInOrder();

            //checks to see if there's entities
            if (entities.Count == 0)
            {
                this.UserFacade.Result = new() { InfoMessage = Text.Original("No entities were selected") };
                return;
            }

            //iterates entities
            foreach (var entity in entities)
            {
                var hub = new Result<T>();
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
                    hub.ErrorMessage = ex.GetMessage();
                }
                finally
                {
                    this.UserFacade.Result = hub;
                }
            }

            await ReloadDataGrid();
            SelectedEntity = Repository.ReadFromCache().Id(SelectedEntity.Id).Clone();
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<GridData<T>> GetEntities(GridState<T> state, CancellationToken token)
        {
            var orders = new StringBuilder();

            if (state.SortDefinitions.Any() == true)
            {
                //iterates through every sort definition
                foreach (var definition in state.SortDefinitions)
                {
                    orders.AppendFormat(",{0} {1}",
                        definition.SortBy,
                        definition.Descending ? "DESC" : "ASC");
                }
            }
            else
            {
                if (DefaultOrdering.Has() == true)
                {
                    orders.Append($",{DefaultOrdering}");
                }
            }

            var filters = new StringBuilder("(true)");

            //needs to filter by all, published or drafts
            if (this is IFilter filter)
            {
                if (filter.Filter.Like("published") == true)
                {
                    if (typeof(T).Implements<IPublished>() == true)
                    {
                        filters.Append($" && ({nameof(IPublished.IsPublished)} == true)");
                    }
                }
                else if (filter.Filter.Like("drafts") == true)
                {
                    if (typeof(T).Implements<IDraft>() == true)
                    {
                        filters.Append($" && ({nameof(IDraft.IsDraft)} == true)");
                    }
                }
            }

            //iterates through every filter definition
            foreach (var definition in state.FilterDefinitions)
            {
                filters.AppendFormat(" && ({0})", definition.Filter());
            }

            var orderBy = orders.ToString();

            //pulls information from cache
            var result = GetItems().Query(
                state.PageSize,
                state.Page * state.PageSize,
                filters.ToString(),
                orderBy[1..]
            );

            return Task.FromResult(new GridData<T> { Items = result.Rows, TotalItems = result.NumberOfRows, });
        }

        /// <summary>
        /// Retrieves all the items for the DataGrid.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<T> GetItems()
        {
            IEnumerable<T> items = Repository.ReadFromCache();

            if (IncludeDeletedEntitiesInDataGrid == false)
            {
                items = items.NonDeleted();
            }

            if (typeof(T).Implements<IBlogId>() == true)
            {
                var query = from a in items.Cast<IBlogId>()
                            where a.BlogId == this.UserFacade.BlogId
                            select a;

                items = query.Cast<T>();
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
                    await ReloadDataGrid();
                    await this.InvokeAsync(this.StateHasChanged);
                }
            };
        }

        /// <summary>
        /// User selects an <paramref name="entity"/> to edit
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnEdit(T entity)
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
            await ReloadDataGrid();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            HasBlogChangedInUserFacade();
        }

        protected virtual void OnSearchResultSelected(T entity)
        {
            SelectedEntity = entity.Clone();
        }

        /// <summary>
        /// Purges the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T> PurgeEntity(T entity)
        {
            var context = new DataOperationContext<T>(this.UserFacade.User, DateTime.UtcNow, entity);
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
        protected virtual async Task ReloadDataGrid()
        {
            SelectedEntity = CreateEntity();
            SelectedEntities = new();
            await DataGrid!.ReloadServerData();
        }

        /// <summary>
        /// Clean <paramref name="entity"/> from cache and its children
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual Result<T> RemoveEntityFromCache(T entity)
        {
            Repository.PurgeCache(entity);
            SelectedEntity = SelectedEntity.Id == entity.Id ? CreateEntity() : SelectedEntity;
            return new(entity);
        }

        /// <summary>
        /// Restores the entity and its children (if appropriate)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual Result<T> RestoreEntity(T entity)
        {
            var context = new DataOperationContext<T>(this.UserFacade.User, DateTime.UtcNow, entity);
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

        protected virtual string RowClassFunc(T entity, int index)
        {
            var result = string.Empty;
            if (entity.Id == SelectedEntity.Id) result += " selected-row";
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
        protected virtual List<T> SelectedEntitiesInOrder()
        {
            return SelectedEntities.ToList();
        }
    }
}
