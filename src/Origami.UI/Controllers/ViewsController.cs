using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;
using UAParser;

namespace Origami.UI.Controllers
{
    [Route("views")]
    public class ViewsController : Controller
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMyMemoryCache _myMemoryCache;
        protected readonly IPhysicalPageRepository _physicalPage;
        protected readonly IPhysicalPageViewRepository _physicalPageView;
        protected readonly ISuperRepository _superRepository;
        protected readonly IUserFacade _userFacade;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="post"></param>
        public ViewsController(
            IMyMemoryCache memoryCache,
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IHttpContextAccessor httpContextAccessor,
            IPhysicalPageRepository physicalPage,
            IPhysicalPageViewRepository physicalPageView,
            ISuperRepository superRepository,
            IUserFacade userFacade
            )
            : base()
        {
            _appFacade = appFacade;
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _myMemoryCache = memoryCache;
            _physicalPage = physicalPage;
            _physicalPageView = physicalPageView;
            _superRepository = superRepository;
            _userFacade = userFacade;
        }

        [HttpGet]
        [Route("physicalpages/{id:guid}")]
        public IActionResult PhysicalPages([FromRoute] Guid id, [FromQuery] string url, [FromQuery] string referrer)
        {
            var page = _physicalPage.ReadFromCache().FirstOrDefault(x => x.Id == id);
            if (page != null)
            {
                var view = new OrigamiPhysicalPageView
                {
                    Id = Guid.NewGuid(),
                    PhysicalPageId = page.Id,
                    Admin = _appFacade.Admin.GetValueOrDefault(),
                };

                this._fill(view, url, referrer);
                _physicalPageView.SmartSave(view.GetContext(), false);

                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        [Route("physicalpages/bycontent")]
        public IActionResult PhysicalPagesByContent([FromQuery] string path, [FromQuery] string type, [FromQuery] string id, [FromQuery] string url, [FromQuery] string referrer)
        {
            if (path.Has() == false) path = "/";

            using var db = this._dbContextFactory.CreateDbContext();
            var pages = from p in db.Set<OrigamiPhysicalPage>().AsNoTracking() where p.Path.Equals(path) == true select p;

            var page = pages.FirstOrDefault();
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = path,
                    DateCreated = DateTime.UtcNow,
                };

                using (var transaction = new TransactionScope())
                {
                    var result = _physicalPage.SmartSave(page.GetContext(), false);
                    if (result.Ok)
                    {
                        transaction.Complete();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }

            if (page != null)
            {
                var view = new OrigamiPhysicalPageView
                {
                    Id = Guid.NewGuid(),
                    PhysicalPageId = page.Id,
                    Admin = _appFacade.Admin,
                    ContentId = Guid.Parse(id),
                };
                this._fill(view, url, referrer);
                this._physicalPageView.SmartSave(view.GetContext(), false);
                this._appFacade.RefreshUI(this.HttpContext.Connection.Id, OrigamiConstants.Events.UpdateCounters);
                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        [Route("physicalpages/bypath")]
        public IActionResult PhysicalPagesByPath([FromQuery] string path, [FromQuery] string url, [FromQuery] string referrer)
        {
            if (path.Has() == false) path = "/";

            using var db = this._dbContextFactory.CreateDbContext();

            var page = db.Set<OrigamiPhysicalPage>().AsNoTracking().FirstOrDefault(x => x.Path.Equals(path) == true);
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = path,
                    DateCreated = DateTime.UtcNow
                };
                using (var transaction = new TransactionScope())
                {
                    var result = _physicalPage.SmartSave(page.GetContext(), false);
                    if (result.Ok)
                    {
                        transaction.Complete();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            if (page != null)
            {
                var view = new OrigamiPhysicalPageView
                {
                    Id = Guid.NewGuid(),
                    PhysicalPageId = page.Id,
                    Admin = _appFacade.Admin,
                    ContentId = null,
                };
                this._fill(view, url, referrer);
                this._physicalPageView.SmartSave(view.GetContext(), false);
                this._appFacade.RefreshUI(this.HttpContext.Connection.Id, OrigamiConstants.Events.UpdateCounters);
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Fills the <paramref name="tracking"/> with request information
        /// </summary>
        /// <param name="tracking"></param>
        /// <param name="url"></param>
        /// <param name="referrer"></param>
        private void _fill(BaseTracking tracking, string url, string referrer)
        {
            var dd = Request.GetDeviceDetector();

            // important!
            dd.Parse();

            tracking.DateCreated = DateTime.UtcNow;
            tracking.Url = url;
            tracking.UrlReferrer = referrer;
            tracking.UserAgent = HttpContext.Request.Header("User-Agent");
            tracking.HostAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            tracking.IsMobileDevice = dd.IsTablet() || dd.IsMobile();
            tracking.IsBot = dd.IsBot();
            tracking.SocialProfileId = _userFacade.SocialProfile.New == false ? _userFacade.SocialProfile.Id : null;

            var client = Parser.GetDefault().Parse(tracking.UserAgent);

            tracking.Platform = client.OS.Family;
            tracking.Browser = client.UA.Family;

            var key = $"Origami_UserLocation_{this.HttpContext.Connection.Id}";
            tracking.Location = this._myMemoryCache.Get<Location>(key);

            if (tracking is OrigamiPhysicalPageView ppv)
            {
                var user = this.HttpContext.Items["loggedin-admin-user"] as OrigamiUser;
                if (user != null)
                {
                    ppv.UserId = user.Id;
                }
            }
        }
    }
}
