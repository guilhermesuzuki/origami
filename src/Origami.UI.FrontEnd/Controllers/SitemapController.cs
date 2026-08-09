using Microsoft.AspNetCore.Mvc;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class SitemapController : Controller
    {
        private string[] sites = ["bing", "google"];

        [HttpGet("{site}")]
        public IActionResult Index(string site)
        {
            site = site.ToLower();

            if (sites.Contains(site) == false)
            {
                return NotFound();
            }

            Response.ContentType = "application/xml";
            return View("Sitemap", site);
        }
    }
}
