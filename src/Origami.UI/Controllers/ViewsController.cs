using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
    public class ViewsController(
            IAppFacade appFacade,
            IDbContextFactory<OrigamiDbContext> _dbContextFactory,
            IHttpContextAccessor _httpContextAccessor,
            IMyMemoryCache memoryCache,
            IPhysicalPageRepository _physicalPage,
            IPhysicalPageViewRepository physicalPageView,
            ISuperRepository _superRepository,
            IUserFacade _userFacade,
            TimeProvider chronos
            ) : Controller
    {
        [HttpGet]
        [Route("physicalpages/bycontent")]
        public IActionResult PhysicalPagesByContent([FromQuery] string path, [FromQuery] string url, [FromQuery] Guid contentId, [FromQuery] Guid? userId, [FromQuery] Guid? socialProfileId)
        {
            if (path.Has() == false) path = "/";

            using var db = _dbContextFactory.CreateDbContext();
            var pages = from p in db.Set<OrigamiPhysicalPage>().AsNoTracking() where p.Path.Equals(path) == true select p;

            var page = pages.FirstOrDefault();
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = path,
                    DateCreated = chronos.GetUtcNow().UtcDateTime,
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
                    Admin = appFacade.Admin,
                    ContentId = contentId,
                };

                _fill(view, url, userId, socialProfileId);
                physicalPageView.SmartSave(view.GetContext(), false);
                appFacade.RefreshUI(HttpContext.Connection.Id, OrigamiConstants.Events.UpdateCounters);

                return Ok(new RequestContext
                {
                    ConnectionId = HttpContext.Connection.Id,
                    Headers = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    Host = HttpContext.Request.Host.Value,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Referrer = HttpContext.Request.Headers.Referer.ToString(),
                    Scheme = HttpContext.Request.Scheme,
                    UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                });
            }

            return NotFound();
        }

        [HttpGet]
        [Route("physicalpages/bypath")]
        public IActionResult PhysicalPagesByPath([FromQuery] string path, [FromQuery] string url, [FromQuery] Guid? userId, [FromQuery] Guid? socialProfileId)
        {
            if (path.Has() == false) path = "/";

            using var db = _dbContextFactory.CreateDbContext();

            var page = db.Set<OrigamiPhysicalPage>().AsNoTracking().FirstOrDefault(x => x.Path.Equals(path) == true);
            if (page == null)
            {
                page = new()
                {
                    Id = Guid.NewGuid(),
                    Path = path,
                    DateCreated = chronos.GetUtcNow().UtcDateTime,
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
                    Admin = appFacade.Admin,
                    ContentId = null,
                };

                _fill(view, url, userId, socialProfileId);
                physicalPageView.SmartSave(view.GetContext(), false);
                appFacade.RefreshUI(HttpContext.Connection.Id, OrigamiConstants.Events.UpdateCounters);

                return Ok(new RequestContext 
                { 
                    ConnectionId = HttpContext.Connection.Id,
                    Headers = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                    Host = HttpContext.Request.Host.Value,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Referrer = HttpContext.Request.Headers.Referer.ToString(),
                    Scheme = HttpContext.Request.Scheme,
                    UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                });
            }

            return NotFound();
        }

        /// <summary>
        /// Fills the <paramref name="tracking"/> with request information
        /// </summary>
        /// <param name="tracking"></param>
        /// <param name="url"></param>
        /// <param name="userId"></param>
        /// <param name="socialProfileId"></param>
        private void _fill(BaseTracking tracking, string url, Guid? userId = null, Guid? socialProfileId = null)
        {
            var dd = Request.GetDeviceDetector();

            // important!
            dd.Parse();

            tracking.DateCreated = DateTime.UtcNow;
            tracking.Url = url;
            tracking.UserAgent = HttpContext.Request.Headers.UserAgent.ToString();
            tracking.HostAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            tracking.IsMobileDevice = dd.IsTablet() || dd.IsMobile();
            tracking.IsBot = dd.IsBot();
            tracking.SocialProfileId = _userFacade.SocialProfile.New == false ? _userFacade.SocialProfile.Id : null;

            var client = Parser.GetDefault().Parse(tracking.UserAgent);

            tracking.Platform = client.OS.Family;
            tracking.Browser = client.UA.Family;

            var key = $"Origami_UserLocation_{HttpContext.Connection.Id}";
            tracking.Location = memoryCache.Get<Location>(key);

            var user = memoryCache.Read<OrigamiUser>().FirstOrDefault(x => x.Id == userId);
            tracking.UserId = user?.Id;

            var socialProfile = memoryCache.Read<OrigamiSocialProfile>().FirstOrDefault(x => x.Id == socialProfileId);
            tracking.SocialProfileId = socialProfile?.Id;
        }
    }
}
