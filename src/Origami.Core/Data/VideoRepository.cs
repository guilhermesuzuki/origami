using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class VideoRepository :
        RepositoryOuterLayer<OrigamiVideo>,
        IVideoRepository
    {
        protected readonly IValidator<OrigamiVideo> _validator;
        protected readonly IVideoCategoryRepository _videoCategoryRepository;
        protected readonly IVideoCommentRepository _videoCommentRepository;
        protected readonly IVideoRatingRepository _videoRatingRepository;
        protected readonly IVideoTagRepository _videoTagRepository;
        protected readonly IVideoViewRepository _videoViewRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public VideoRepository(
            IValidator<OrigamiVideo> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IVideoCategoryRepository videoCategoryRepository,
            IVideoCommentRepository videoCommentRepository,
            IVideoRatingRepository videoRatingRepository,
            IVideoTagRepository videoTagRepository,
            IVideoViewRepository videoViewRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _videoCategoryRepository = videoCategoryRepository;
            _videoCommentRepository = videoCommentRepository;
            _videoRatingRepository = videoRatingRepository;
            _videoTagRepository = videoTagRepository;
            _videoViewRepository = videoViewRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewVideos);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteOtherUsersVideos);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteOwnVideos);
        public override string PublishOtherUsersPermission => nameof(OrigamiRole.PublishOtherUsersVideos);
        public override string PublishOwnPermission => nameof(OrigamiRole.PublishOwnVideos);
        public override string PurgePermission => nameof(OrigamiRole.PurgeVideos);
        public override string ReadPermission => nameof(OrigamiRole.ViewVideos);
        public override string RestorePermission => nameof(OrigamiRole.RestoreVideos);
        public override string UnpublishOtherUsersPermission => nameof(OrigamiRole.UnpublishOtherUsersVideos);
        public override string UnpublishOwnPermission => nameof(OrigamiRole.UnpublishOwnVideos);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsersVideos);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnVideos);

        public override Result<OrigamiVideo> CreateValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            var validation = new Result<OrigamiVideo>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }

        public override void PurgeRelationshipsFromCache(OrigamiVideo entity)
        {
            base.PurgeRelationshipsFromCache(entity);

            var categories = _videoCategoryRepository.ReadFromCache().Where(x => x.VideoId == entity.Id);
            var comments = _videoCommentRepository.ReadFromCache().Where(x => x.VideoId == entity.Id);
            var ratings = _videoRatingRepository.ReadFromCache().Where(x => x.VideoId == entity.Id);
            var tags = _videoTagRepository.ReadFromCache().Where(x => x.VideoId == entity.Id);



            categories.Each(_videoCategoryRepository.PurgeCache);
            comments.Each(_videoCommentRepository.PurgeCache);
            ratings.Each(_videoRatingRepository.PurgeCache);
            tags.Each(_videoTagRepository.PurgeCache);
        }

        public override Result<OrigamiVideo> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiVideo> ctx)
        {
            var hub = base.PurgeRelationshipsFromDatabase(ctx);

            var categories = _videoCategoryRepository.ReadFromDatabase().Where(x => x.VideoId == ctx.Entity.Id).WithOnlyIds();
            var comments = _videoCommentRepository.ReadFromDatabase().Where(x => x.VideoId == ctx.Entity.Id).WithOnlyIds();
            var ratings = _videoRatingRepository.ReadFromDatabase().Where(x => x.VideoId == ctx.Entity.Id).WithOnlyIds();
            var tags = _videoTagRepository.ReadFromDatabase().Where(x => x.VideoId == ctx.Entity.Id).WithOnlyIds();

            categories.GetContexts(ctx).Call(_videoCategoryRepository.SmartPurge, false).Push(hub);
            comments.GetContexts(ctx).Call(_videoCommentRepository.SmartPurge, false).Push(hub);
            ratings.GetContexts(ctx).Call(_videoRatingRepository.SmartPurge, false).Push(hub);
            tags.GetContexts(ctx).Call(_videoTagRepository.SmartPurge, false).Push(hub);

            hub.RowsAffected += _videoViewRepository.ReadFromDatabase().Where(x => x.VideoId == ctx.Entity.Id).ExecuteDelete();

            return hub;
        }

        public override Result<OrigamiVideo> UpdateValidation(DataOperationContext<OrigamiVideo> ctx)
        {
            var validation = new Result<OrigamiVideo>(ctx.Entity, _validator);
            this.ValidateSlug(ctx).Push(validation);
            return validation;
        }
    }
}
