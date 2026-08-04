using Microsoft.AspNetCore.Mvc;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class SitemapController : Controller
    {
        string[] sites = [ "bing", "google" ];

        [HttpGet("{site}")]
        public IActionResult Index(string site)
        {
            if (sites.Contains(site) == false)
            {
                return NotFound();
            }
            Response.ContentType = "application/xml";
            return View("Sitemap", site);
        }
    }
}
