using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System.Linq.Dynamic.Core;

namespace Origami.Core.Data
{
    public class PostRepository : RepositoryOuterLayer<OrigamiPost>, IPostRepository
    {
        protected readonly IPostCategoryRepository _postCategoryRepository;
        protected readonly IPostCommentRepository _postCommentRepository;
        protected readonly IPostRatingRepository _postRatingRepository;
        protected readonly IPostTagRepository _postTagRepository;
        protected readonly IPostViewRepository _postViewRepository;
        protected readonly IValidator<OrigamiPost> _validator;
        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PostRepository(
            IValidator<OrigamiPost> validator,
            IPostCategoryRepository postCategoryRepository,
            IPostCommentRepository postCommentRepository,
            IPostRatingRepository postRatingRepository,
            IPostTagRepository postTagRepository,
            IPostViewRepository postViewRepository,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _postCategoryRepository = postCategoryRepository;
            _postCommentRepository = postCommentRepository;
            _postRatingRepository = postRatingRepository;
            _postTagRepository = postTagRepository;
            _postViewRepository = postViewRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewPosts);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersPosts);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnPosts);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersPosts);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnPosts);
        public override string PurgePermission => nameof(OrigamiRole.PurgePosts);
        public override string ReadPermission => nameof(OrigamiRole.ViewPosts);
        public override string RestorePermission => nameof(OrigamiRole.RestorePosts);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersPosts);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnPosts);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersPosts);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnPosts);

        public override Result<OrigamiPost> CreateValidation(DataOperationContext<OrigamiPost> ctx)
        {
            var validation = new Result<OrigamiPost>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }

        public override void PurgeRelationshipsFromCache(OrigamiPost entity)
        {
            base.PurgeRelationshipsFromCache(entity);

            var categories = _postCategoryRepository.ReadFromCache().Where(x => x.PostId == entity.Id);
            var comments = _postCommentRepository.ReadFromCache().Where(x => x.PostId == entity.Id);
            var ratings = _postRatingRepository.ReadFromCache().Where(x => x.PostId == entity.Id);
            var tags = _postTagRepository.ReadFromCache().Where(x => x.PostId == entity.Id);

            categories.Each(_postCategoryRepository.PurgeCache);
            comments.Each(_postCommentRepository.PurgeCache);
            ratings.Each(_postRatingRepository.PurgeCache);
            tags.Each(_postTagRepository.PurgeCache);
        }

        public override Result<OrigamiPost> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiPost> ctx)
        {
            var hub = base.PurgeRelationshipsFromDatabase(ctx);

            var categories = _postCategoryRepository.ReadFromDatabase().Where(x => x.PostId == ctx.Entity.Id).WithOnlyIds();
            var comments = _postCommentRepository.ReadFromDatabase().Where(x => x.PostId == ctx.Entity.Id).WithOnlyIds();
            var ratings = _postRatingRepository.ReadFromDatabase().Where(x => x.PostId == ctx.Entity.Id).WithOnlyIds();
            var tags = _postTagRepository.ReadFromDatabase().Where(x => x.PostId == ctx.Entity.Id).WithOnlyIds();

            categories.GetContexts(ctx).Call(_postCategoryRepository.SmartPurge, false).Push(hub);
            comments.GetContexts(ctx).Call(_postCommentRepository.SmartPurge, false).Push(hub);
            ratings.GetContexts(ctx).Call(_postRatingRepository.SmartPurge, false).Push(hub);
            tags.GetContexts(ctx).Call(_postTagRepository.SmartPurge, false).Push(hub);

            hub.RowsAffected += _postViewRepository.ReadFromDatabase().Where(x => x.PostId == ctx.Entity.Id).ExecuteDelete();

            return hub;
        }
        public override Result<OrigamiPost> UpdateValidation(DataOperationContext<OrigamiPost> ctx)
        {
            var validation = new Result<OrigamiPost>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }
    }
}
