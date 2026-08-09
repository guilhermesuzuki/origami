using Microsoft.AspNetCore.Mvc;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class SitemapController : Controller
    {
        private static readonly string[] Sites = ["bing", "google"];

        [HttpGet("{site}")]
        public IActionResult Index(string site)
        {
            if (Sites.Contains(site, StringComparer.OrdinalIgnoreCase) == false)
            {
                return NotFound();
            }

            Response.ContentType = "application/xml";
            return View("Sitemap", site);
        }
    }
}
