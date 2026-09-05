using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
                var createResult = this.SmartSave(physicalPage.GetContext(), false);
                if (createResult.Ok == false) return new Result<OrigamiPhysicalPageView>(view).Pull(createResult);
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

            var hub = this._physicalPageViewRepository.SmartSave(view.GetContext(), false);

            if (hub.Ok)
            {
                lock (OrigamiConstants.SyncRoot)
                {
                    var count = this.MemoryCache.TryGetValue<long>(virtualPath, out long x) ? x : 0L;
                    this.MemoryCache.Set(virtualPath, ++count);
                }
            }

            return hub;
        }

        public long Views(string virtualPath)
        {
            return this.MemoryCache.TryGetValue<long>(virtualPath, out long x) ? x : 0L;
        }
    }
}
