using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class UserRepository :
        RepositoryOuterLayer<OrigamiUser>,
        IUserRepository
    {
        protected readonly IContentRepository _contentRepository;
        protected readonly IUserBlogRepository _userBlogRepository;
        protected readonly IUserPasswordResetRepository _userPasswordResetRepository;
        protected readonly IUserRoleRepository _userRoleRepository;
        protected readonly IValidator<OrigamiUser> _validator;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public UserRepository(
            IAppFacade appFacade,
            IValidator<OrigamiUser> validator,
            IContentRepository contentRepository,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IUserBlogRepository userBlogRepository,
            IUserPasswordResetRepository userPasswordResetRepository,
            IUserRoleRepository userRoleRepository,
            Text text,
            IWebRootPath wwwRoot)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _validator = validator;
            _contentRepository = contentRepository;
            _userBlogRepository = userBlogRepository;
            _userPasswordResetRepository = userPasswordResetRepository;
            _userRoleRepository = userRoleRepository;
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

            var fresh = this.ReadFromDatabase(ctx.Entity);
            if (fresh is { IsBlocked: true })
            {
                // TODO: add this to resx files
                return new() { Error = Text.Original("User is already blocked") };
            }

            ctx.Entity.IsBlocked = true;
            ctx.Entity.DateBlocked = DateTime.UtcNow;

            return this.SmartUpdate(ctx, false);
        }

        public bool CanTheUserModerateComments(IId user)
        {
            return this.CheckPermission(user.Id, nameof(OrigamiRole.ModerateComments)).Ok;
        }

        public Result<OrigamiUser> ChangePassword(DataOperationContext<OrigamiUser> ctx, string oldPassword, string newPassword1, string newPassword2)
        {
            ctx.Entity.NewPassword1 = newPassword1;
            ctx.Entity.NewPassword2 = newPassword2;

            using var db = DbContextFactory.CreateDbContext();

            // this is necessary because of ReadFromDatabase
            var hash = oldPassword.SHA256Hash();

            var fresh = db.Set<OrigamiUser>().AsNoTracking()
                .Where(x => x.Username.ToLower() == ctx.Entity.Username.ToLower())
                .Where(x => x.Password == hash)
                .FirstOrDefault();

            if (fresh == null) return new() { Error = Text.Original("Username and current password do NOT exist in the database") };
            if (newPassword1 != newPassword2) return new() { Error = Text.Original("New passwords do NOT match, they differ from each other") };
            if (oldPassword == newPassword1) return new() { Error = Text.Original("You did NOT change passwords, current and new are the same") };

            var hub = new Result<OrigamiUser>(ctx.Entity).Pull(newPassword1.IsPasswordStrong(Text));
            if (hub.Ok == false) return hub;

            // sets the new password
            ctx.Entity.MustChangePassword = false;
            ctx.Entity.Password = newPassword1.SHA256Hash();

            if (hub.Ok == true)
            {
                base.SmartUpdate(ctx, false).Push(hub);
            }

            return hub;
        }

        public override Result<OrigamiUser> Create(DataOperationContext<OrigamiUser> ctx)
        {
            var hub = new Result<OrigamiUser>(ctx.Entity);

            ctx.Entity.MustChangePassword = true;
            ctx.Entity.Password = ctx.Entity.NewPassword1.SHA256Hash();

            hub.Info = Text.Original("A password has been created: {0}", ctx.Entity.NewPassword1);
            hub.Password = ctx.Entity.NewPassword1;

            base.Create(ctx).Push(hub);

            ctx.Entity.UserBlogs.Each(ub => ub.UserId = ctx.Entity.Id);
            ctx.Entity.UserRoles.Each(ur => ur.UserId = ctx.Entity.Id);

            ctx.Entity.UserBlogs.GetContexts(ctx).Each(x => this._userBlogRepository.SmartSave(x, false).Push(hub));
            ctx.Entity.UserRoles.GetContexts(ctx).Each(x => this._userRoleRepository.SmartSave(x, false).Push(hub));

            return hub;
        }

        public override Result<OrigamiUser> CreateValidation(DataOperationContext<OrigamiUser> ctx)
        {
            return new(ctx.Entity, _validator);
        }

        public Result<string> ForgotOwnPassword(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwnPassword));
                if (permission.Ok == false)
                {
                    var hub = new Result<string>
                    {
                        Error = Text.Original("You don't have permission to reset your own password"),
                        Simple = Text.Original("Please, talk to a system administrator")
                    };
                    return hub;
                }
            }

            var password = "@"
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Letters, size: 4)
                + Nanoid.Generate(alphabet: Nanoid.Alphabets.Digits, size: 4)
                + "#";

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
                    Error = Text.Original("Failed to reset password for user")
                };
            }

            var fresh = this.ReadFromDatabase(ctx.Entity);

            ctx.Entity.MustChangePassword = true;
            ctx.Entity.Password = password.SHA256Hash();
            ctx.Entity.Version(fresh);

            this.UpdateCache(ctx.Entity);

            return new(password) { RowsAffected = row };
        }

        public OrigamiUser? LookupUserInDatabase(string username, string cleanPassword)
        {
            username = username.ToLower().Trim();
            var password = cleanPassword.SHA256Hash();

            using var db = DbContextFactory.CreateDbContext();

            var query = from x in db.Set<OrigamiUser>().AsNoTracking().NonDeleted()
                        where x.IsDeleted == false
                        where x.IsBlocked == false
                        where x.Username.ToLower() == username
                        where x.Password == password
                        select x;

            return query.FirstOrDefault();
        }

        public override void PurgeRelationshipsFromCache(OrigamiUser entity)
        {
            // Purge roles from cache
            var contents = _contentRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();
            var resets1 = _userPasswordResetRepository.ReadFromCache().Where(x => x.UserId == entity.Id).ToList();
            var resets2 = _userPasswordResetRepository.ReadFromCache().Where(x => x.AuthorId == entity.Id).ToList();
            var roles = from x in _userRoleRepository.ReadFromCache() where x.UserId == entity.Id select x;
            var userBlogs = _userBlogRepository.ReadFromCache().Where(x => x.UserId == entity.Id);

            contents.Each(this._contentRepository.PurgeCache);
            resets1.Each(this._userPasswordResetRepository.PurgeCache);
            resets2.Each(this._userPasswordResetRepository.PurgeCache);
            roles.Each(this._userRoleRepository.PurgeCache);
            userBlogs.Each(this._userBlogRepository.PurgeCache);
        }

        public override Result<OrigamiUser> PurgeRelationshipsFromDatabase(DataOperationContext<OrigamiUser> ctx)
        {
            var hub = new Result<OrigamiUser>(ctx.Entity);

            using (var db = DbContextFactory.CreateDbContext())
            {
                var contents = db.Contents.AsNoTracking().Where(x => x.AuthorId == ctx.Entity.Id).ToList();

                contents.GetContexts(ctx).Call(_contentRepository.SmartPurge, false).Push(hub);

                var del1 = db.UserRoles.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();
                var del2 = db.UserPasswordResets.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();
                var del3 = db.UserPasswordResets.Where(x => x.AuthorId == ctx.Entity.Id).ExecuteDelete();
                var del4 = db.UserBlogs.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();
                var del5 = db.PhysicalPageViews.Where(x => x.UserId == ctx.Entity.Id).ExecuteDelete();

                hub.RowsAffected += del1;
                hub.RowsAffected += del2;
                hub.RowsAffected += del3;
                hub.RowsAffected += del4;
                hub.RowsAffected += del5;
            }

            return hub;
        }

        public Result Reset2FA(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = ctx.Entity.Id == ctx.User.Id
                    ? this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwn2FA))
                    : this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOtherUsers2FA));

                if (permission.Ok == false)
                {
                    permission.Error = Text.Original("You don't have permission to reset 2FA");
                    permission.Simple = Text.Original("Please, talk to a system administrator");
                    return permission;
                }
            }

            var fresh = this.ReadFromDatabase(ctx.Entity);
            if (fresh != null)
            {
                ctx.Entity.TOTPSecret = string.Empty;
                ctx.Entity.TOTPRecoveryCodes = string.Empty;
                return this.SmartUpdate(ctx, false);
            }

            return new() { Error = Text.Original("Failed to reset 2FA for user") };
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

                    hub.Error = Text.Original("You don't have permission to reset password");
                    hub.Simple = Text.Original("Please, talk to a system administrator");

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

            _userPasswordResetRepository.CreateCache(reset);

            return new(reset.Key)
            {
                RowsAffected = row,
                Simple = Text.Original("Password reset link has been created, please check your email for further instructions")
            };
        }

        public Result ResetPassword(DataOperationContext<OrigamiUser> ctx, string key, string newPassword1, string newPassword2, bool checkPermission)
        {
            if (ctx.Entity.Id != ctx.User.Id)
            {
                // TODO: add this to resx files
                return new() { Error = Text.Original("You can only reset your own password") };
            }

            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.ResetOwnPassword));
                if (permission.Ok == false) return permission;
            }

            var hub = newPassword1.IsPasswordStrong(Text);
            if (hub.Ok)
            {
                if (newPassword1 != newPassword2)
                {
                    hub.Error = Text.Original("New passwords do NOT match, they differ from each other");
                }
            }

            if (hub.Ok)
            {
                using var db = DbContextFactory.CreateDbContext();

                var users = from x in db.Set<OrigamiUser>().AsNoTracking().NonDeleted()
                            where x.IsBlocked == false
                            where x.Id == ctx.Entity.Id
                            select x;

                var user = users.FirstOrDefault();
                if (user != null)
                {
                    var resets = from x in db.UserPasswordResets.AsNoTracking()
                                 where x.Key == key
                                 where x.UserId == ctx.Entity.Id
                                 where x.IsDeleted == false
                                 select x;

                    var reset = resets.FirstOrDefault();
                    if (reset != null)
                    {
                        reset.IsDeleted = true;
                        db.Update(reset);
                        db.SaveChanges();

                        _userPasswordResetRepository.UpdateCache(reset);

                        ctx.Entity.Password = newPassword1.SHA256Hash();
                        this.SmartUpdate(ctx, false).Push(hub);

                        hub.Success = Text.Original("Password has been reset successfully");
                        return hub;
                    }

                    // TODO: add this to resx files
                    hub.Error = Text.Original("Password reset key is invalid or has already been used");
                }
            }

            hub.Error = Text.Original("Failed to reset password");
            hub.Simple = Text.Original("Please, try again later");

            return hub;
        }

        public Result<OrigamiUser> Unblock(DataOperationContext<OrigamiUser> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = CheckPermission(ctx, UnblockUsersPermission);
                if (permission.Ok == false) return permission;
            }

            var fresh = this.ReadFromDatabase(ctx.Entity);
            if (fresh is { IsBlocked: false })
            {
                // TODO: add this to resx files
                return new() { Error = Text.Original("User is already unblocked") };
            }

            ctx.Entity.IsBlocked = false;
            ctx.Entity.DateUnblocked = DateTime.UtcNow;

            return this.SmartUpdate(ctx, false);
        }

        public override Result<OrigamiUser> Update(DataOperationContext<OrigamiUser> ctx)
        {
            if (ctx.Entity.NewPassword1.Has() == true
                && ctx.Entity.NewPassword2.Has() == true
                && ctx.Entity.NewPassword1 == ctx.Entity.NewPassword2)
            {
                ctx.Entity.Password = ctx.Entity.NewPassword1.SHA256Hash();
            }

            var hub = base.Update(ctx);

            ctx.Entity.UserBlogs.Each(ub => ub.UserId = ctx.Entity.Id);
            ctx.Entity.UserRoles.Each(ur => ur.UserId = ctx.Entity.Id);

            using var db = this.DbContextFactory.CreateDbContext();
            var dbo1 = db.Set<OrigamiUserRole>().AsNoTracking().Where(x => x.UserId == ctx.Entity.Id).ToList();
            var merge1 = dbo1.GetMerge(ctx.Entity.UserRoles);
            hub.OnSuccess(() => this._userRoleRepository.Merge(ctx, merge1).Push(hub));
            var dbo2 = db.Set<OrigamiUserBlog>().AsNoTracking().Where(x => x.UserId == ctx.Entity.Id).ToList();
            var merge2 = dbo2.GetMerge(ctx.Entity.UserBlogs);
            hub.OnSuccess(() => this._userBlogRepository.Merge(ctx, merge2).Push(hub));

            return hub;
        }

        public override Result<OrigamiUser> UpdateValidation(DataOperationContext<OrigamiUser> ctx)
        {
            return new(ctx.Entity, _validator);
        }
    }
}
