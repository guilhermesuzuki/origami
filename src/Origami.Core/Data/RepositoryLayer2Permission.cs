using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public abstract class RepositoryLayer2Permission<T> :
        RepositoryLayer1Validation<T>,
        ICreatePermission<T>,
        IReadPermission<T>,
        IUpdatePermission<T>,
        IDeletePermission<T>,
        IPublishPermission<T>
        where T : class, IId, new()
    {
        protected RepositoryLayer2Permission(
            Text text,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IWebRootPath webRootPath)
            : base(text, dbContextFactory, memoryCache, webRootPath)
        {

        }

        public virtual string BlockUserSelfPermission => nameof(OrigamiRole.BlockUserSelf);
        public virtual string BlockUsersOtherThanSelfPermission => nameof(OrigamiRole.BlockUsersOtherThanSelf);
        public virtual string CreatePermission => throw new NotImplementedException();
        public virtual string DeleteOtherUsersPermission => throw new NotImplementedException();
        public virtual string DeleteOwnPermission => throw new NotImplementedException();
        public virtual string DeletePermission => throw new NotImplementedException();
        public virtual string PublishOtherUsersPermission => throw new NotImplementedException();
        public virtual string PublishOwnPermission => throw new NotImplementedException();
        public virtual string PurgePermission => throw new NotImplementedException();
        public virtual string ReadPermission => throw new NotImplementedException();
        public virtual string RestorePermission => throw new NotImplementedException();
        public virtual string UnblockUsersPermission => nameof(OrigamiRole.UnblockUsers);
        public virtual string UnpublishOtherUsersPermission => throw new NotImplementedException();
        public virtual string UnpublishOwnPermission => throw new NotImplementedException();
        public virtual string UpdateOtherUsersPermission => throw new NotImplementedException();
        public virtual string UpdateOwnPermission => throw new NotImplementedException();
        public virtual string UpdatePermission => throw new NotImplementedException();

        public virtual Result<T> CanBlock(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is OrigamiUser user)
            {
                if (ctx.Entity.Id == ctx.User.Id)
                {
                    return CheckPermission(ctx, BlockUserSelfPermission);
                }
                else
                {
                    return CheckPermission(ctx, BlockUsersOtherThanSelfPermission);
                }
            }
            throw new NotImplementedException();
        }

        public virtual Result<T> CanCreate(DataOperationContext<T> ctx)
        {
            return this.CheckPermission(ctx, CreatePermission);
        }

        public virtual Result<T> CanDelete(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is IAuthorId author1 && author1.AuthorId == ctx.User.Id)
            {
                return CheckPermission(ctx, DeleteOwnPermission);
            }
            if (ctx.Entity is IAuthorId author2 && author2.AuthorId != ctx.User.Id)
            {
                return CheckPermission(ctx, DeleteOtherUsersPermission);
            }
            if (ctx.Entity is OrigamiUser user1 && user1.Id == ctx.User.Id)
            {
                return CheckPermission(ctx, DeleteOwnPermission);
            }
            if (ctx.Entity is OrigamiUser user2 && user2.Id != ctx.User.Id)
            {
                return CheckPermission(ctx, DeleteOtherUsersPermission);
            }

            return this.CheckPermission(ctx, DeletePermission);
        }

        public virtual Result<T> CanPublish(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is IAuthorId author1 && author1.AuthorId == ctx.User.Id)
            {
                return CheckPermission(ctx, PublishOwnPermission);
            }
            if (ctx.Entity is IAuthorId author2 && author2.AuthorId != ctx.User.Id)
            {
                return CheckPermission(ctx, PublishOtherUsersPermission);
            }

            throw new NotImplementedException();
        }

        public virtual Result<T> CanPurge(DataOperationContext<T> ctx)
        {
            return this.CheckPermission(ctx, PurgePermission);
        }

        public virtual Result CanRead(Guid userId)
        {
            return this.CheckPermission(userId, ReadPermission);
        }

        public virtual Result<T> CanRestore(DataOperationContext<T> ctx)
        {
            return this.CheckPermission(ctx, RestorePermission);
        }

        public Result<T> CanUnpublish(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is IAuthorId author1 && author1.AuthorId == ctx.User.Id)
            {
                return CheckPermission(ctx, UnpublishOwnPermission);
            }
            if (ctx.Entity is IAuthorId author2 && author2.AuthorId != ctx.User.Id)
            {
                return CheckPermission(ctx, UnpublishOtherUsersPermission);
            }

            throw new NotImplementedException();
        }

        public virtual Result<T> CanUpdate(DataOperationContext<T> ctx)
        {
            if (ctx.Entity is IAuthorId author1 && author1.AuthorId == ctx.User.Id)
            {
                return CheckPermission(ctx, UpdateOwnPermission);
            }
            if (ctx.Entity is IAuthorId author2 && author2.AuthorId != ctx.User.Id)
            {
                return CheckPermission(ctx, UpdateOtherUsersPermission);
            }
            if (ctx.Entity is OrigamiUser user1 && user1.Id == ctx.User.Id)
            {
                return CheckPermission(ctx, UpdateOwnPermission);
            }
            if (ctx.Entity is OrigamiUser user2 && user2.Id != ctx.User.Id)
            {
                return CheckPermission(ctx, UpdateOtherUsersPermission);
            }

            return this.CheckPermission(ctx, UpdatePermission);
        }

        #region Permission methods

        protected virtual Result CheckPermission(Guid userId, string permission)
        {
            var result = new Result() { InfoMessage = permission, };
            if (UserHasPermission(userId, permission) == true) return result;
            result.ErrorMessage = Text.Original(Text.YouDontHavePermissionForThisFeature);
            return result;
        }

        protected virtual Result<T> CheckPermission(DataOperationContext<T> ctx, string permission)
        {
            var result = new Result<T>(ctx.Entity) { InfoMessage = permission, };
            if (UserHasPermission(ctx.User.Id, permission) == true) return result;
            result.ErrorMessage = Text.Original(Text.YouDontHavePermissionForThisFeature);
            return result;
        }

        protected virtual bool UserHasPermission(Guid userId, string permission)
        {
            using var db = DbContextFactory.CreateDbContext();

            var user = db.Users.Id(userId);
            if (user == null) return false;
            if (user.IsDeleted) return false;
            if (user.IsBlocked) return false;

            var query = from us in db.Users
                        join ur in db.UserRoles on us.Id equals ur.UserId
                        join ro in db.Roles on ur.RoleId equals ro.Id
                        join rr in db.RightRoles on ro.Id equals rr.RoleId
                        join ri in db.Rights on rr.RightId equals ri.Id
                        where us.IsDeleted == false
                        where ro.IsDeleted == false
                        where us.Id == userId && ri.Name == permission
                        select 1;

            return query.Any();
        }

        #endregion
    }
}
