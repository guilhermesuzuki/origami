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

        public Result<OrigamiPhysicalPageView> View(string virtualPath, OrigamiPhysicalPageView view, OrigamiSocialProfile socialProfile)
        {
            var physicalPage = this.ReadFromCache().FirstOrDefault(x => x.Path == virtualPath);
            if (physicalPage == null)
            {
                physicalPage = new() { Path = virtualPath, DateCreated = DateTime.UtcNow };
                this.SmartSave(physicalPage.GetContext(), false);
            }
            view.PhysicalPageId = physicalPage.Id;
            view.SocialProfileId = socialProfile.Id;
            this._physicalPageViewRepository.SmartSave(view.GetContext(), false);
            return new(view);
        }

        public Result<OrigamiPhysicalPageView> View(string virtualPath, OrigamiPhysicalPageView view, OrigamiUser user)
        {
            var physicalPage = this.ReadFromCache().FirstOrDefault(x => x.Path == virtualPath);
            if (physicalPage == null)
            {
                physicalPage = new() { Path = virtualPath, DateCreated = DateTime.UtcNow };
                this.SmartSave(physicalPage.GetContext(), false);
            }
            view.PhysicalPageId = physicalPage.Id;
            view.UserId = user.Id;
            this._physicalPageViewRepository.SmartSave(view.GetContext(), false);
            return new(view);
        }
    }
}
