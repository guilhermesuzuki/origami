using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NanoidDotNet;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserRepository :
        RepositoryOuterLayer<OrigamiUser>,
        IUserRepository
    {
        protected readonly IPageRepository _pageRepository;
        protected readonly IPostRepository _postRepository;
        protected readonly IUserPasswordResetRepository _userPasswordResetRepository;
        protected readonly IUserRoleRepository _userRoleRepository;
        protected readonly IValidator<OrigamiUser> _validator;
        protected readonly IVideoRepository _videoRepository;


        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public UserRepository(
            IValidator<OrigamiUser> validator,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IPageRepository pageRepository,
            IPostRepository postRepository,
            IUserPasswordResetRepository userPasswordResetRepository,
            IUserRoleRepository userRoleRepository,
            IVideoRepository videoRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _validator = validator;
            _pageRepository = pageRepository;
            _postRepository = postRepository;
            _userPasswordResetRepository = userPasswordResetRepository;
            _userRoleRepository = userRoleRepository;
            _videoRepository = videoRepository;
        }

        public override string CreatePermission => nameof(OrigamiRole.CreateNewUsers);
        public override string DeleteOtherUsersPermission => nameof(OrigamiRole.DeleteUsersOtherThanSelf);
        public override string DeleteOwnPermission => nameof(OrigamiRole.DeleteUserSelf);
        public override string PurgePermission => nameof(OrigamiRole.PurgeUsers);
        public override string ReadPermission => nameof(OrigamiRole.ViewUsers);
        public override string RestorePermission => nameof(OrigamiRole.RestoreUsers);
        public override string UpdateOtherUsersPermission => nameof(OrigamiRole.EditOtherUsers);
        public override string UpdateOwnPermission => nameof(OrigamiRole.EditOwnUser);

        public Result<OrigamiUser> Block(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CanBlock(ctx);
                if (permission.Ok == false) return permission;
            }

            ctx.Entity.IsBlocked = true;
            ctx.Entity.DateBlocked = DateTime.UtcNow;

            return this.SmartUpdate(ctx, false);
        }

        public Result<OrigamiUser> ChangePassword(DataOperationContext<OrigamiUser> ctx, string oldPassword, string newPassword1, string newPassword2)
        {
            // this is necessary because of ReadFromDatabase
            var hash = oldPassword.SHA256Hash();

            var user = ReadFromDatabase()
                .Where(x => x.Username.ToLower() == ctx.Entity.Username.ToLower())
                .Where(x => x.Password == hash)
                .FirstOrDefault();

            if (user == null) return new() { ErrorMessage = Text.Original("Username and current password do NOT exist in the database") };
            if (newPassword1 != newPassword2) return new() { ErrorMessage = Text.Original("New passwords do NOT match, they differ from each other") };
            if (oldPassword == newPassword1) return new() { ErrorMessage = Text.Original("You did NOT change passwords, current and new are the same") };

            var result = new Result<OrigamiUser>(user).Pull(newPassword1.IsPasswordStrong());
            if (result.Ok == false) return result;

            // sets the new password
            user.MustChangePassword = false;
            user.Password = newPassword1.SHA256Hash();

            var userContext = new DataOperationContext<OrigamiUser>(ctx.User, ctx.DateTime, user);
            if (result.Ok == true) base.SmartUpdate(userContext, false).Push(result);

            return result;
        }

        public override Result<OrigamiUser> CreateValidation(DataOperationContext<OrigamiUser> ctx)
        {
            var validation = new Result<OrigamiUser>(ctx.Entity, _validator);
            return validation;
        }

        public Result<string> ForgotOwnPassword(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwnPassword));
                if (permission.Ok == false)
                {
                    var hub = new Result<string>();

                    hub.ErrorMessage = Text.Original("You don't have permission to reset your own password");
                    hub.SimpleMessage = Text.Original("Please, talk to a system administrator");

                    return hub;
                }
            }

            var password = Nanoid.Generate(size: 8);

            using var db = DbContextFactory.CreateDbContext();
            var row = db.Users.Where(x => x.Id == ctx.Entity.Id).ExecuteUpdate(
                setters => setters
                    .SetProperty(x => x.Password, password.SHA256Hash())
                    .SetProperty(x => x.MustChangePassword, true)
            );

            if (row != 1)
            {
                return new()
                {
                    RowsAffected = row,
                    ErrorMessage = Text.Original("Failed to reset password for user")
                };
            }

            return new(password) { RowsAffected = row };
        }

        public OrigamiUser? LookupUserInDatabase(string username, string cleanPassword)
        {
            username = username.ToLower().Trim();
            var hash = cleanPassword.SHA256Hash();

            var query = from x in ReadFromDatabase().NonDeleted()
                        where x.IsDeleted == false
                        where x.IsBlocked == false
                        where x.Username.ToLower() == username
                        where x.Password == hash
                        select x;

            return query.FirstOrDefault();
        }

        public override void PurgeRelationshipsFromCache(OrigamiUser entity)
        {
            // Purge roles from cache
            var pages = _pageRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();
            var posts = _postRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();
            var resets1 = _userPasswordResetRepository.ReadFromCache().Where(x => x.UserId == entity.Id).ToList();
            var resets2 = _userPasswordResetRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();
            var roles = from x in _userRoleRepository.ReadFromCache() where x.UserId == entity.Id select x;
            var videos = _videoRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();

            pages.Each(this._pageRepository.PurgeCache);
            posts.Each(this._postRepository.PurgeCache);
            roles.Each(this._userRoleRepository.PurgeCache);
            videos.Each(this._videoRepository.PurgeCache);

            resets1.Each(this._userPasswordResetRepository.PurgeCache);
            resets2.Each(this._userPasswordResetRepository.PurgeCache);
        }

        public override Result<OrigamiUser> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiUser> ctx)
        {
            var hub = new Result<OrigamiUser>(ctx.Entity);

            var pages = _pageRepository.ReadFromDatabase().Where(x => x.AuthorId == ctx.Entity.Id).ToList();
            var posts = _postRepository.ReadFromDatabase().Where(x => x.AuthorId == ctx.Entity.Id).ToList();
            var videos = _videoRepository.ReadFromDatabase().Where(x => x.AuthorId == ctx.Entity.Id).ToList();

            pages.GetContexts(ctx).Call(_pageRepository.SmartPurge, false).Push(hub);
            posts.GetContexts(ctx).Call(_postRepository.SmartPurge, false).Push(hub);
            videos.GetContexts(ctx).Call(_videoRepository.SmartPurge, false).Push(hub);

            using (var db = DbContextFactory.CreateDbContext())
            {
                var del1 = db.UserRoles.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();
                var del2 = db.UserPasswordResets.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();
                var del3 = db.UserPasswordResets.Where(x => x.AuthorId == ctx.Entity.Id).ExecuteDelete();
                hub.RowsAffected += del1;
                hub.RowsAffected += del2;
                hub.RowsAffected += del3;
            }

            return hub;
        }

        public Result<string> ResetPassword(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = ctx.Entity.Id == ctx.User.Id
                    ? this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwnPassword))
                    : this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOtherUsersPasswords));

                if (permission.Ok == false)
                {
                    var hub = new Result<string>();

                    hub.ErrorMessage = Text.Original("You don't have permission to reset password");
                    hub.SimpleMessage = Text.Original("Please, talk to a system administrator");

                    return hub;
                }
            }

            var reset = new OrigamiUserPasswordReset
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                UserId = ctx.Entity.Id,
                AuthorId = ctx.User.Id,
            };

            using var db = DbContextFactory.CreateDbContext();
            db.Add(reset);
            var row = db.SaveChanges();

            return new(reset.Key)
            {
                RowsAffected = row,
                SimpleMessage = Text.Original("Password reset link has been created, please check your email for further instructions")
            };
        }

        public Result ResetPassword(DataOperationContext<OrigamiUser> ctx, string key, string newPassword1, string newPassword2, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwnPassword));
                if (permission.Ok == false) return permission;
            }

            var hub = newPassword1.IsPasswordStrong();
            if (hub.Ok)
            {
                if (newPassword1 != newPassword2)
                {
                    hub.ErrorMessage = Text.Original("New passwords do NOT match, they differ from each other");
                }
            }

            if (hub.Ok)
            {
                var user = from x in this.ReadFromDatabase().NonDeleted()
                           where x.IsBlocked == false
                           where x.Id == ctx.Entity.Id
                           select x;

                var userEntity = user.FirstOrDefault();

                if (userEntity != null)
                {
                    using var db = DbContextFactory.CreateDbContext();
                    var reset = from x in db.UserPasswordResets
                                where x.Key == key
                                where x.UserId == ctx.Entity.Id
                                where x.IsDeleted == false
                                select x;

                    var resetEntity = reset.FirstOrDefault();
                    if (resetEntity != null)
                    {
                        resetEntity.IsDeleted = true;
                        userEntity.Password = newPassword1.SHA256Hash();
                        this.SmartUpdate(userEntity.GetContext(ctx.User), false).Push(hub);
                        db.Update(resetEntity);
                        db.SaveChanges();
                        hub.SuccessMessage = Text.Original("Password has been reset successfully");
                        return hub;
                    }
                }
            }

            hub.ErrorMessage = Text.Original("Failed to reset password");
            hub.SimpleMessage = Text.Original("Please, try again later");

            return hub;
        }

        public Result<OrigamiUser> Unblock(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, UnblockUsersPermission);
                if (permission.Ok == false) return permission;
            }

            ctx.Entity.IsBlocked = false;
            ctx.Entity.DateUnblocked = DateTime.UtcNow;

            return this.SmartUpdate(ctx, false);
        }
        public override Result<OrigamiUser> UpdateValidation(DataOperationContext<OrigamiUser> ctx)
        {
            var validation = new Result<OrigamiUser>(ctx.Entity, _validator);
            return validation;
        }
    }
}
