using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin
{
    public class BasicFormFullContent2<T1, T2> : 
        BasicForm<T2>
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        [Inject] public IHubContentRepository<T2> HubContentRepository { get; set; } = null!;
        [Inject] public IMemoryCache MemoryCache { get; set; } = null!;
        
        protected MudDataGrid<OrigamiCategory> CategoriesGrid { get; set; } = null!;
        protected string CategoriesSearch { get; set; } = string.Empty;
        protected MudDataGrid<OrigamiTag> TagsGrid { get; set; } = null!;
        protected string TagsSearch { get; set; } = string.Empty;

        public override void Save()
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var hub = HubContentRepository.Save(Entity, UserFacade.User);
                    if (hub.Ok)
                    {
                        transaction.Complete();
                    }
                    this.UserFacade.Result = hub;
                    this.Saved.InvokeAsync(Entity).Wait();
                }
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
            }
        }

        public override void UndoChanges()
        {
            this.Entity = HubContentRepository.Get(Entity.Entity);
        }

        /// <summary>
        /// Associates the specified category with the current entity if it is not already associated.
        /// </summary>
        /// <remarks>If the category is already associated with the entity, this method has no
        /// effect.</remarks>
        /// <param name="category">The category to add to the entity. Cannot be null.</param>
        protected void AddCategory(OrigamiCategory category)
        {
            var query = Entity.Categories.Where(x => x.CategoryId == category.Id);
            if (query.Any() == false)
            {
                Entity.Categories.Add(new() { CategoryId = category.Id, ContentId = Entity.Entity.Id });
            }
        }

        /// <summary>
        /// Adds a tag to the entity if it does not already exist.
        /// </summary>
        /// <remarks>If the specified tag is already associated with the entity, this method does nothing.
        /// Tag comparison is based on the tag's value.</remarks>
        /// <param name="tag">The tag to add to the entity. Cannot be null.</param>
        protected void AddTag(OrigamiTag tag)
        {
            var query = Entity.Tags.Where(x => x.Tag == tag.Tag);
            if (query.Any() == false)
            {
                Entity.Tags.Add(new() { ContentId = Entity.Entity.Id, Tag = tag.Tag });
            }
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<GridData<OrigamiCategory>> GetCategories(GridState<OrigamiCategory> state, CancellationToken token)
        {
            using var db = this.DbContextFactory.CreateDbContext();

            var filter = new StringBuilder("(true)");

            filter.Append($" && {nameof(OrigamiCategory.BlogId)} == {Entity.Entity.BlogId}");
            filter.Append($" && {nameof(OrigamiCategory.Name)}.Contains(\"{CategoriesSearch}\", StringComparison.InvariantCultureIgnoreCase)", CategoriesSearch.Has());

            //pulls information from cache
            var result = db.ReadFromCache<OrigamiCategory>(this.MemoryCache).Query(
                state.PageSize,
                state.Page * state.PageSize,
                filter.ToString(),
                $"{nameof(OrigamiCategory.Name)}"
            );

            return Task.FromResult(new GridData<OrigamiCategory> { Items = result.Rows, TotalItems = result.NumberOfRows, });
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual Task<GridData<OrigamiTag>> GetTags(GridState<OrigamiTag> state, CancellationToken token)
        {
            using var db = this.DbContextFactory.CreateDbContext();

            var filter = new StringBuilder("(true)");

            filter.Append($" && {nameof(OrigamiTag.BlogId)} == {Entity.Entity.BlogId}");
            filter.Append($" && {nameof(OrigamiTag.Tag)}.Contains(\"{TagsSearch}\", StringComparison.InvariantCultureIgnoreCase)", TagsSearch.Has());

            //pulls information from cache
            var result = db.ReadFromCache<OrigamiTag>(this.MemoryCache).Query(
                state.PageSize,
                state.Page * state.PageSize,
                filter.ToString(),
                $"{nameof(OrigamiTag.Tag)}"
            );

            return Task.FromResult(new GridData<OrigamiTag> { Items = result.Rows, TotalItems = result.NumberOfRows, });
        }
    }
}
