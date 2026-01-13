using Microsoft.AspNetCore.Components;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;

namespace Origami.UI.Admin
{
    /// <summary>
    /// Basic Form Full Content for Posts, Videos, etc.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TCat"></typeparam>
    /// <typeparam name="TTag"></typeparam>
    public abstract class BasicFormFullContent<T, TCat, TTag> :
        BasicFormFull<T>
        where T : BaseContent, IId, new()
        where TCat : ICategoryId, IId
        where TTag : ITag, IId
    {
        protected List<TCat> Categories = new();
        protected List<TTag> Tags = new();

        [Inject] public IRepository<TCat> Category { get; set; } = null!;
        [Inject] public IRepository<TTag> Tag { get; set; } = null!;

        protected override bool DisableTheSaveButton
        {
            get
            {
                if (this.Entity == null) return true;
                if (this.Entity.Title.Has() == false) return true;
                return false;
            }
        }

        public override void Save()
        {
            var beforeSaving = BeforeSaving();
            if (beforeSaving.Ok == false)
            {
                UserFacade.Result = beforeSaving;
                return;
            }

            var hub = new Result<T>(this.Entity);
            var ctx = new DataOperationContext<T>(this.UserFacade.User, DateTime.UtcNow, this.Entity);

            try
            {
                using (var transaction = new TransactionScope())
                {
                    hub.OnSuccess(() => Repository.SmartSave(ctx, true).Push(hub));
                    if (Entity.New)
                    {
                        hub.OnSuccess(() => CreateCategories(Categories).Push(hub));
                        hub.OnSuccess(() => CreateTags(Tags).Push(hub));
                    }
                    else
                    {
                        var tagsFromDb = GetTagsFromDb();
                        var categoriesFromDb = GetCategoriesFromDb();

                        var mergeTags = tagsFromDb.GetMergeTags(Tags);
                        var mergeCategories = categoriesFromDb.GetMergeCategories(Categories);

                        hub.OnSuccess(() => this.PurgeCategories(mergeCategories.Purge).Push(hub));
                        hub.OnSuccess(() => this.UpdateCategories(mergeCategories.Update).Push(hub));
                        hub.OnSuccess(() => this.CreateCategories(mergeCategories.Create).Push(hub));
                        hub.OnSuccess(() => this.PurgeTags(mergeTags.Purge).Push(hub));
                        hub.OnSuccess(() => this.UpdateTags(mergeTags.Update).Push(hub));
                        hub.OnSuccess(() => this.CreateTags(mergeTags.Create).Push(hub));
                    }
                    if (hub.Ok)
                    {
                        transaction.Complete();
                    }
                }
                Saved.InvokeAsync(this.Entity).Wait();
            }
            catch (Exception ex)
            {
                hub.ErrorMessage = Text.Get(Text.SomethingWentWrongPleaseTryAgain);
                hub.ErrorMessage = ex.GetMessage();
            }
            finally
            {
                UserFacade.Result = hub;
            }
        }

        public override void UndoChanges()
        {
            base.UndoChanges();
            LoadCategories();
            LoadTags();
        }

        protected override Result<T> BeforeSaving()
        {
            var result = base.BeforeSaving();
            Tags = Tags.DistinctBy(x => x.Tag).ToList();
            return result;
        }

        protected virtual Result<T> CreateCategories(IEnumerable<TCat> create)
        {
            var result = new Result<T>(this.Entity);

            foreach (var category in create)
            {
                switch (category)
                {
                    case OrigamiPostCategory postCategory:
                        postCategory.PostId = this.Entity.Id;
                        break;
                    case OrigamiVideoCategory videoCategory:
                        videoCategory.VideoId = this.Entity.Id;
                        break;
                }
                var context = new DataOperationContext<TCat>(this.UserFacade.User, DateTime.UtcNow, category);
                Category.SmartSave(context, false).Push(result);
            }

            return result;
        }

        protected virtual Result<T> CreateTags(IEnumerable<TTag> create)
        {
            var result = new Result<T>(this.Entity);

            foreach (var tag in create)
            {
                switch (tag)
                {
                    case OrigamiPostTag postTag:
                        postTag.PostId = this.Entity.Id;
                        break;
                    case OrigamiVideoTag videoTag:
                        videoTag.VideoId = this.Entity.Id;
                        break;
                }
                var context = new DataOperationContext<TTag>(this.UserFacade.User, DateTime.UtcNow, tag);
                Tag.SmartSave(context, false).Push(result);
            }

            return result;
        }

        protected virtual IEnumerable<OrigamiCategory> GetAllCategories()
        {
            return from x in this.Super.Categories.ReadFromCache().Blog(this.UserFacade.BlogId).NonDeleted()
                   where this.Super.IsParentDeleted(x) == false
                   orderby x.Name
                   select x;
        }

        /// <summary>
        /// Retrieves all the <typeparamref name="TCat"/> from a <typeparamref name="T"/> saved in the database.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TCat> GetCategoriesFromDb()
        {
            IEnumerable<TCat> categoriesFromDb = [];

            switch (Category)
            {
                case IRepository<OrigamiPostCategory> postCategories:
                    categoriesFromDb = postCategories.ReadFromDatabase().Where(x => x.PostId == this.Entity.Id).Cast<TCat>().ToList();
                    break;
                case IRepository<OrigamiVideoCategory> videoCategories:
                    categoriesFromDb = videoCategories.ReadFromDatabase().Where(x => x.VideoId == this.Entity.Id).Cast<TCat>().ToList();
                    break;
            }

            return categoriesFromDb;
        }
        /// <summary>
        /// Retrieves all the <typeparamref name="TTag"/> from a <typeparamref name="T"/> saved in the database.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<TTag> GetTagsFromDb()
        {
            IEnumerable<TTag> tagsFromDb = [];

            switch (Tag)
            {
                case IRepository<OrigamiPostTag> postTags:
                    tagsFromDb = postTags.ReadFromDatabase().Where(x => x.PostId == this.Entity.Id).Cast<TTag>().ToList();
                    break;
                case IRepository<OrigamiVideoTag> videoTags:
                    tagsFromDb = videoTags.ReadFromDatabase().Where(x => x.VideoId == this.Entity.Id).Cast<TTag>().ToList();
                    break;
            }

            return tagsFromDb;
        }

        protected virtual bool IsCategorySelected(Guid categoryId)
        {
            return Categories.Any(x => x.CategoryId == categoryId);
        }

        /// <summary>
        /// This method should load all categories
        /// </summary>
        protected virtual void LoadCategories()
        {
            Categories.Clear();

            if (this.Entity == null) return;
            if (this.Entity.New == true) return;

            switch (Category)
            {
                case IRepository<OrigamiPostCategory> postCategories:
                    Categories = postCategories.ReadFromCache().Where(x => x.PostId == this.Entity.Id).Cast<TCat>().ToList();
                    break;
                case IRepository<OrigamiVideoCategory> videoCategories:
                    Categories = videoCategories.ReadFromCache().Where(x => x.VideoId == this.Entity.Id).Cast<TCat>().ToList();
                    break;
            }
        }

        /// <summary>
        /// This method should load all tags
        /// </summary>
        protected virtual void LoadTags()
        {
            Tags.Clear();

            if (this.Entity == null) return;
            if (this.Entity.New == true) return;

            switch (Tag)
            {
                case IRepository<OrigamiPostTag> postTags:
                    Tags = postTags.ReadFromCache().Where(x => x.PostId == this.Entity.Id).Cast<TTag>().ToList();
                    break;
                case IRepository<OrigamiVideoTag> videoTags:
                    Tags = videoTags.ReadFromCache().Where(x => x.VideoId == this.Entity.Id).Cast<TTag>().ToList();
                    break;
            }
        }

        protected bool NewCategory(Guid categoryId)
        {
            if (IsCategorySelected(categoryId) == false) return false;

            switch (Category)
            {
                case IRepository<OrigamiPostCategory> postCategories:
                    return postCategories.ReadFromCache().Where(x => x.CategoryId == categoryId).Where(x => x.PostId == this.Entity.Id).Any() == false;
                case IRepository<OrigamiVideoCategory> videoCategories:
                    return videoCategories.ReadFromCache().Where(x => x.CategoryId == categoryId).Where(x => x.VideoId == this.Entity.Id).Any() == false;
            }

            throw new NotImplementedException();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            LoadCategories();
            LoadTags();
        }

        protected virtual Result<T> PurgeCategories(IEnumerable<TCat> purge)
        {
            var result = new Result<T>(this.Entity);

            foreach (var category in purge)
            {
                var context = new DataOperationContext<TCat>(this.UserFacade.User, DateTime.UtcNow, category);
                Category.SmartPurge(context, false).Push(result);
            }

            return result;
        }

        protected virtual Result<T> PurgeTags(IEnumerable<TTag> purge)
        {
            var result = new Result<T>(this.Entity);

            foreach (var tag in purge)
            {
                var context = new DataOperationContext<TTag>(this.UserFacade.User, DateTime.UtcNow, tag);
                Tag.SmartPurge(context, false).Push(result);
            }

            return result;
        }

        /// <summary>
        /// Removes the tag from the collection
        /// </summary>
        /// <param name="tag"></param>
        protected virtual void RemoveTag(TTag tag)
        {
            Tags.Remove(tag);
        }
        protected virtual void SelectCategory(OrigamiCategory category)
        {
            if (IsCategorySelected(category.Id) == true)
            {
                Categories.RemoveAll(x => x.CategoryId == category.Id);
            }
            else
            {
                var entity = Activator.CreateInstance<TCat>();

                entity.CategoryId = category.Id;

                if (entity is IFKPost fkPost) fkPost.PostId = Entity.Id;
                if (entity is IFKVideo fkVideo) fkVideo.VideoId = Entity.Id;

                Categories.Add(entity);
            }
        }

        protected void TagEntered(string tagName)
        {
            var tag = Activator.CreateInstance<TTag>();
            if (tag != null)
            {
                tag.Tag = tagName;
                Tags.Add(tag);
            }
        }

        protected virtual Result<T> UpdateCategories(IEnumerable<TCat> update)
        {
            var result = new Result<T>(this.Entity);

            foreach (var category in update)
            {
                var context = new DataOperationContext<TCat>(this.UserFacade.User, DateTime.UtcNow, category);

                if (category is IDeleted deleted && deleted.IsDeleted)
                {
                    Category.SmartDelete(context, false).Push(result);
                }
                else
                {
                    Category.SmartSave(context, false).Push(result);
                }
            }

            return result;
        }

        protected virtual Result<T> UpdateTags(IEnumerable<TTag> update)
        {
            var result = new Result<T>(this.Entity);

            foreach (var tag in update)
            {
                var context = new DataOperationContext<TTag>(this.UserFacade.User, DateTime.UtcNow, tag);

                if (tag is IDeleted deleted && deleted.IsDeleted)
                {
                    Tag.SmartDelete(context, false).Push(result);
                }
                else
                {
                    Tag.SmartSave(context, false).Push(result);
                }
            }

            return result;
        }
    }
}
