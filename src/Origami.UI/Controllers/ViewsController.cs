using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Origami.Core;
using Origami.Core.Data;
using Origami.Core.Models;
using System.Transactions;
using UAParser;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("views")]
    public class ViewsController : Controller
    {
        protected readonly IAppFacade _appFacade;
        protected readonly IDbContextFactory<OrigamiDbContext> _dbContextFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IPageRepository _page;
        protected readonly IPhysicalPageRepository _physicalPage;
        protected readonly IPhysicalPageViewRepository _physicalPageView;
        protected readonly IPostRepository _post;
        protected readonly ISuperRepository _superRepository;
        protected readonly IUserFacade _userFacade;
        protected readonly IVideoRepository _video;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="post"></param>
        public ViewsController(
            IMemoryCache memoryCache,
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> dbContextFactory,
            IHttpContextAccessor httpContextAccessor,
            IPageRepository page,
            IPhysicalPageRepository physicalPage,
            IPhysicalPageViewRepository physicalPageView,
            IPostRepository post,
            ISuperRepository superRepository,
            IUserFacade userFacade,
            IVideoRepository video)
            : base()
        {
            _appFacade = appFacade;
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _page = page;
            _physicalPage = physicalPage;
            _physicalPageView = physicalPageView;
            _post = post;
            _superRepository = superRepository;
            _userFacade = userFacade;
            _video = video;
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
                    Content = new()
                    {
                        Type = type,
                        Id = Guid.Parse(id)
                    },
                };
                this._fill(view, url, referrer);
                _physicalPageView.SmartSave(view.GetContext(), false);
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
                    Content = new()
                    {
                        Type = nameof(OrigamiPhysicalPage),
                        Id = Guid.Empty
                    },
                };
                this._fill(view, url, referrer);
                _physicalPageView.SmartSave(view.GetContext(), false);
                return Ok();
            }

            return NotFound();
        }

        /// <summary>
        /// Fills the <paramref name="view"/> with request information
        /// </summary>
        /// <param name="view"></param>
        /// <param name="url"></param>
        /// <param name="referrer"></param>
        private void _fill(BaseView view, string url, string referrer)
        {
            var dd = Request.GetDeviceDetector();

            // important!
            dd.Parse();

            view.DateCreated = DateTime.UtcNow;
            view.Url = url;
            view.UrlReferrer = referrer;
            view.UserAgent = HttpContext.Request.Header("User-Agent");
            view.HostAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            view.IsMobileDevice = dd.IsTablet() || dd.IsMobile();
            view.IsBot = dd.IsBot();
            view.SocialProfileId = _userFacade.SocialProfile.New == false ? _userFacade.SocialProfile.Id : null;

            var client = Parser.GetDefault().Parse(view.UserAgent);

            view.Platform = client.OS.Family;
            view.Browser = client.UA.Family;

            var key = $"Origami_UserLocation_{this._httpContextAccessor.HttpContext?.Connection.Id}";
            view.Location = this._memoryCache.Get<Location>(key);

            if (view is OrigamiPhysicalPageView ppv)
            {
                var user = this.HttpContext.Items["loggedIn-admin-user"] as OrigamiUser;
                if (user != null)
                {
                    ppv.UserId = user.Id;
                }
            }
        }
    }
}
