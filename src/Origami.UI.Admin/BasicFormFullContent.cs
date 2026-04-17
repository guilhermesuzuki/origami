using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using MudBlazor;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Text;
using System.Transactions;

namespace Origami.UI.Admin
{
    public class BasicFormFullContent<T1, T2> :
        BasicForm<T2>
        where T1 : OrigamiContent, new()
        where T2 : class, IHubContent<T1>, new()
    {
        [Inject] public IHubContentRepository<T2> HubContentRepository { get; set; } = null!;

        protected List<OrigamiCategory> Categories { get; set; } = [];
        protected List<OrigamiContentTag> Tags { get; set; } = [];

        protected string CategoriesSearch { get; set; } = string.Empty;
        protected string TagsSearch { get; set; } = string.Empty;

        public override void Save()
        {
            var hub = new Result<T2>(this.Entity);

            try
            {
                using (var transaction = new TransactionScope())
                {
                    HubContentRepository.Save(Entity, UserFacade.User).Push(hub);
                    if (hub.Ok)
                    {
                        transaction.Complete();
                    }
                    this.UserFacade.Result = hub;
                }
                hub.OnSuccess(() => Saved.InvokeAsync(hub.Entity));
            }
            catch (Exception ex)
            {
                UserFacade.Result = new(ex);
            }
        }

        public override void UndoChanges()
        {
            this.ShowParentSelector = false;
            this.Entity = HubContentRepository.Get(Entity.Entity).Clone();
        }

        /// <summary>
        /// Adds the specified content to the associated collection if it does not already exist.
        /// </summary>
        /// <remarks>If the specified content already exists in the collection, no action is taken. Only
        /// OrigamiCategory and OrigamiTag types are supported; other types are ignored.</remarks>
        /// <typeparam name="T">The type of the content to add. Supported types are OrigamiCategory and OrigamiTag.</typeparam>
        /// <param name="content">The content item to add to the collection. Must be of type OrigamiCategory or OrigamiTag.</param>
        protected void Add<T>(T content)
        {
            if (content is OrigamiCategory category)
            {
                Entity.Categories.Add(new() { CategoryId = category.Id, ContentId = Entity.Entity.Id });
                Entity.Categories = Entity.Categories.DistinctBy(x => x.CategoryId).ToList();
            }
            else if (content is OrigamiContentTag tag)
            {
                Entity.Tags.Add(new() { ContentId = Entity.Entity.Id, Tag = tag.Tag });
                Entity.Tags = Entity.Tags.DistinctBy(x => x.Tag).ToList();
            }
        }

        /// <summary>
        /// This method should retrieve the Entities from database or memory
        /// </summary>
        /// <param name="state"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual List<T> Get<T>() where T : class
        {
            var t = Activator.CreateInstance<T>();
            var query = from c in this.MemoryCache.Read<T>() select c;

            if (t is IBlogIdNull blogIdNull)
            {
                query = query.Where(x => (x as IBlogIdNull)?.BlogId == this.Entity.Entity.BlogId);
            }

            query = t switch
            {
                IName => query.Cast<IName>().OrderBy(x => x.Name).Cast<T>(),
                ITag => query.Cast<ITag>().DistinctBy(x => x.Tag).OrderBy(x => x.Tag).Cast<T>(),
                _ => query,
            };
            return [.. query];
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Entity.Entity.SetAuthor(UserFacade.User);
            Entity.Entity.BlogId = Entity.Entity switch
            {
                OrigamiSpecialMessage => null,
                OrigamiSpecialPage => null,
                _ => GetBlogFromUserFacade().Id,
            };
            Categories = Get<OrigamiCategory>();
            Tags = Get<OrigamiContentTag>();
        }

        protected void SearchCategory(KeyboardEventArgs kea)
        {
            this.Categories = CategoriesSearch.Has() == false ? this.Get<OrigamiCategory>() : this.Get<OrigamiCategory>().Where(x => x.Name.Contains(CategoriesSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            this.InvokeAsync(this.StateHasChanged);
        }

        protected void SearchTag(KeyboardEventArgs kea)
        {
            this.Tags = TagsSearch.Has() == false ? this.Get<OrigamiContentTag>() : [new() { Tag = TagsSearch }, .. this.Get<OrigamiContentTag>().Where(x => x.Tag.Contains(TagsSearch, StringComparison.OrdinalIgnoreCase))];
            this.InvokeAsync(this.StateHasChanged);
        }

        protected override void CreateEntityBeforeEvent(T2 entity)
        {
            entity.Entity.SetAuthor(UserFacade.User);
            entity.Entity.BlogId = entity.Entity switch 
            { 
                OrigamiSpecialMessage => null,
                OrigamiSpecialPage => null,
                _ => GetBlogFromUserFacade().Id,
            };
        }

        public override void SetParent(IId entity)
        {
            this.Entity.Parent = entity as T1;
            this.Entity.Entity.ParentId = entity.Id;
        }

        protected virtual IEnumerable<T1> GetParents()
        {
            return from x in this.MemoryCache.Read<T1>()
                   where x.BlogId == this.UserFacade.BlogId
                   where x.IsDeleted == false
                   where this.Super.IsParentDeleted(x) == false
                   orderby x.Title
                   select x;
        }
    }
}
