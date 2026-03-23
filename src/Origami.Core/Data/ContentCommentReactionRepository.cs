using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Origami.Core.Data
{
    public class ContentCommentReactionRepository:
        RepositoryOuterLayer<OrigamiContentCommentReaction>,
        IContentCommentReactionRepository
    {
        protected readonly IValidator<OrigamiContentCommentReaction> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public ContentCommentReactionRepository(
            IValidator<OrigamiContentCommentReaction> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
        }

        public Result<OrigamiContentCommentReaction> SmartCreate(DataOperationContextFrontEnd<OrigamiContentCommentReaction> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            return base.SmartCreate(ctx, false);
        }

        public Result<OrigamiContentCommentReaction> SmartPurge(DataOperationContextFrontEnd<OrigamiContentCommentReaction> ctx)
        {
            try
            {
                using var db = DbContextFactory.CreateDbContext();
                db.Set<OrigamiSocialProfile>().AsNoTracking().GetProfileThrowIfBlocked(ctx.SocialProfile.Id);
            }
            catch (Exception ex)
            {
                return new(ctx.Entity, ex.GetMessage());
            }

            return base.SmartPurge(ctx, false);
        }

        public IEnumerable<OrigamiContentCommentReaction> Reactions(OrigamiContentComment entity)
        {
            return ReadFromCache().Where(x => x.CommentId == entity.Id);
        }

        public IEnumerable<OrigamiContentCommentReaction> ReactionsFromProfile(OrigamiSocialProfile socialProfile)
        {
            return ReadFromCache().Where(x => x.SocialProfileId == socialProfile.Id);
        }
    }
}
