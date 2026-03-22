using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class SubscriberRepository :
        RepositoryOuterLayer<OrigamiSubscriber>,
        ISubscriberRepository
    {
        protected readonly IEmailRepository _emailRepository;
        protected readonly ISettingsRepository _settingsRepository;

        public SubscriberRepository(
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMemoryCache memoryCache,
            IEmailRepository emailRepository,
            ISettingsRepository settingsRepository,
            Text text,
            IWebRootPath wwwRoot) :
            base(text, dbContextFactory, memoryCache, wwwRoot)
        {
            _emailRepository = emailRepository;
            _settingsRepository = settingsRepository;
        }

        public Result<OrigamiSubscriber> Subscribe(DataOperationContext<OrigamiSocialProfile> ctx)
        {
            //first, it needs to check for email address
            if (ctx.Entity.HasEmail() == false)
            {
                return new() { Error = Text.Original("User did not share an e-mail address") };
            }

            var subscriber = ReadFromCache().Where(x => x.SocialProfileId == ctx.Entity.Id).FirstOrDefault();

            if (subscriber != null)
            {
                subscriber.IsDeleted = false;
                subscriber.DateModified = DateTime.UtcNow;
                subscriber.IsVerified = true;
                subscriber.Email = ctx.Entity.Email;
                var subscribeContext = new DataOperationContext<OrigamiSubscriber>(ctx.User, ctx.DateTime, subscriber);
                return SmartUpdate(subscribeContext, false);
            }
            else
            {
                var newSubscriber = new OrigamiSubscriber
                {
                    Id = Guid.NewGuid(),
                    SocialProfileId = ctx.Entity.Id,
                    DateCreated = DateTime.UtcNow,
                    IsVerified = true,
                    Email = ctx.Entity.Email,
                };
                var subscribeContext = new DataOperationContext<OrigamiSubscriber>(ctx.User, ctx.DateTime, newSubscriber);
                return SmartCreate(subscribeContext, false);
            }
        }

        public Result<OrigamiSubscriber> Unsubscribe(DataOperationContext<OrigamiSocialProfile> ctx, bool checkPermission)
        {
            if (checkPermission)
            {
                var permission = this.CheckPermission(ctx.User.Id, nameof(OrigamiRole.UnsubcribeSocialProfiles));
                if (permission.Ok == false) return new Result<OrigamiSubscriber>().Pull(permission);
            }

            return this.Unsubscribe(ctx);
        }

        public Result<OrigamiSubscriber> Unsubscribe(DataOperationContext<OrigamiSocialProfile> ctx)
        {
            using var db = DbContextFactory.CreateDbContext();
            var subscriber = db.Set<OrigamiSubscriber>().AsNoTracking().Where(x => x.SocialProfileId == ctx.Entity.Id).FirstOrDefault();
            if (subscriber != null)
            {
                var subscribeContext = new DataOperationContext<OrigamiSubscriber>(ctx.User, ctx.DateTime, subscriber);

                subscriber.IsDeleted = true;
                subscriber.DateModified = DateTime.UtcNow;

                return SmartDelete(subscribeContext, false);
            }

            return new() { Error = Text.Original("Social profile is not a subscriber"), };
        }

        public bool ValidateVerificationCode(DataOperationContext<OrigamiSocialProfile> ctx, string code)
        {
            var subscriber = ReadFromCache()
                .Where(x => x.SocialProfileId == ctx.Entity.Id)
                .Where(x => x.VerificationCode == code)
                .FirstOrDefault();

            if (subscriber != null)
            {
                subscriber.IsVerified = true;
                var subscriberContext = new DataOperationContext<OrigamiSubscriber>(ctx.User, ctx.DateTime, subscriber);
                SmartUpdate(subscriberContext, false);
                return true;
            }

            return false;
        }

        public bool VerificationCode(DataOperationContext<OrigamiSocialProfile> ctx, string email, string code)
        {
            var subscriber = ReadFromCache().FirstOrDefault(x => x.SocialProfileId == ctx.Entity.Id) ?? new() { SocialProfileId = ctx.Entity.Id };

            subscriber.IsDeleted = true;
            subscriber.IsVerified = false;
            subscriber.Email = email;
            subscriber.VerificationCode = code;

            var hub = this._emailRepository.SendVerificationCode(email, code);
            if (hub.Ok == false)
            {
                return false;
            }

            var newctx = new DataOperationContext<OrigamiSubscriber>(ctx.User, ctx.DateTime, subscriber);

            SmartSave(newctx, false);

            return true;
        }
    }
}
