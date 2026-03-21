using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IMemoryCache _memoryCache;
        protected readonly IPageRepository _page;
        protected readonly IPageViewRepository _pageView;
        protected readonly IPhysicalPageRepository _physicalPage;
        protected readonly IPhysicalPageViewRepository _physicalPageView;
        protected readonly IPostRepository _post;
        protected readonly IPostViewRepository _postView;
        protected readonly ISuperRepository _superRepository;
        protected readonly IUserFacade _userFacade;
        protected readonly IVideoRepository _video;
        protected readonly IVideoViewRepository _videoView;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="post"></param>
        public ViewsController(
            IMemoryCache memoryCache,
            IAppFacade appFacade,
            IHttpContextAccessor httpContextAccessor,
            IPageRepository page,
            IPageViewRepository pageView,
            IPhysicalPageRepository physicalPage,
            IPhysicalPageViewRepository physicalPageView,
            IPostRepository post,
            IPostViewRepository postView,
            ISuperRepository superRepository,
            IUserFacade userFacade,
            IVideoRepository video,
            IVideoViewRepository videoView)
            : base()
        {
            _memoryCache = memoryCache;
            _appFacade = appFacade;
            _httpContextAccessor = httpContextAccessor;
            _page = page;
            _pageView = pageView;
            _physicalPage = physicalPage;
            _physicalPageView = physicalPageView;
            _post = post;
            _postView = postView;
            _superRepository = superRepository;
            _userFacade = userFacade;
            _video = video;
            _videoView = videoView;
        }

        [HttpGet]
        [Route("pages/{id:guid}")]
        public IActionResult Pages([FromRoute] Guid id, [FromQuery] string url, [FromQuery] string referrer)
        {
            var page = _page.ReadFromCache().FirstOrDefault(x => x.Id == id);
            if (page != null)
            {
                var view = new OrigamiPageView
                {
                    Id = Guid.NewGuid(),
                    PageId = page.Id,
                };

                this._fill(view, url, referrer);
                _pageView.SmartSave(view.GetContext(), false);

                return Ok();
            }

            return NotFound();
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

            var pages = from p in _physicalPage.ReadFromDatabase()
                        where p.Path.Equals(path) == true
                        select p;

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

            var page = _physicalPage.ReadFromDatabase().FirstOrDefault(x => x.Path.Equals(path) == true);
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
                };
                this._fill(view, url, referrer);
                _physicalPageView.SmartSave(view.GetContext(), false);
                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        [Route("posts/{id:guid}")]
        public IActionResult Posts([FromRoute] Guid id, [FromQuery] string url, [FromQuery] string referrer)
        {
            var post = _post.ReadFromCache().FirstOrDefault(x => x.Id == id);
            if (post != null)
            {
                var dd = Request.GetDeviceDetector();

                var view = new OrigamiPostView
                {
                    Id = Guid.NewGuid(),
                    PostId = post.Id,
                };

                this._fill(view, url, referrer);
                _postView.SmartSave(view.GetContext(), false);

                return Ok();
            }

            return NotFound();
        }

        [HttpGet]
        [Route("specialpages/{id:guid}")]
        public IActionResult SpecialPages([FromRoute] Guid id, [FromQuery] string url, [FromQuery] string referrer)
        {
            var page = _superRepository.SpecialPages.ReadFromCache().FirstOrDefault(x => x.Id == id);
            if (page != null)
            {
                var view = new OrigamiSpecialPageView
                {
                    Id = Guid.NewGuid(),
                    SpecialPageId = page.Id,
                };

                this._fill(view, url, referrer);
                _superRepository.SpecialPageViews.SmartSave(view.GetContext(), false);

                return Ok();
            }

            return NotFound();
        }
        [HttpGet]
        [Route("videos/{id:guid}")]
        public IActionResult Videos([FromRoute] Guid id, [FromQuery] string url, [FromQuery] string referrer)
        {
            var video = _video.ReadFromCache().FirstOrDefault(x => x.Id == id);
            if (video != null)
            {
                var dd = Request.GetDeviceDetector();

                var view = new OrigamiVideoView
                {
                    Id = Guid.NewGuid(),
                    VideoId = video.Id,
                };

                this._fill(view, url, referrer);
                _videoView.SmartSave(view.GetContext(), false);

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
