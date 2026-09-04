using Microsoft.EntityFrameworkCore;
using Origami.Core.Models;

namespace Origami.Core.Data
{
    public class PhysicalPageRepository :
        RepositoryOuterLayer<OrigamiPhysicalPage>,
        IPhysicalPageRepository
    {
        protected readonly IPhysicalPageViewRepository _physicalPageViewRepository;

        /// <summary>
        /// Default constructor with DI
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="distributedCache"></param>
        public PhysicalPageRepository(
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IMyMemoryCache memoryCache,
            IPhysicalPageViewRepository physicalPageViewRepository,
            IWebRootPath wwwRoot,
            Text text)
            : base(text, dbContextFactory, memoryCache, wwwRoot, appFacade)
        {
            _physicalPageViewRepository = physicalPageViewRepository;
        }

        public Result<OrigamiPhysicalPageView> View<T>(string virtualPath, OrigamiPhysicalPageView view, T whoIsResponsible)
        {
            var physicalPage = this.ReadFromCache().FirstOrDefault(x => x.Path == virtualPath);
            if (physicalPage == null)
            {
                physicalPage = new() { Path = virtualPath, DateCreated = DateTime.UtcNow };
                this.SmartSave(physicalPage.GetContext(), false);
            }

            view.PhysicalPageId = physicalPage.Id;

            if (whoIsResponsible is OrigamiUser user && user.Id != Guid.Empty)
            {
                view.UserId = user.Id;
            }
            else if (whoIsResponsible is OrigamiSocialProfile socialProfile && socialProfile.Id != Guid.Empty)
            {
                view.SocialProfileId = socialProfile.Id;
            }

            return this._physicalPageViewRepository.SmartSave(view.GetContext(), false);
        }

        public long Views(string virtualPath)
        {
            var physicalPage = this.ReadFromCache().FirstOrDefault(x => x.Path == virtualPath);
            if (physicalPage != null)
            {
                using var db = this.DbContextFactory.CreateDbContext();
                var query = db.PhysicalPageViews.Where(x => x.PhysicalPageId == physicalPage.Id);
                return query.LongCount();
            }
            return 0L;
        }
    }
}
