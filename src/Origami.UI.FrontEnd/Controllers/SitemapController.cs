using Microsoft.AspNetCore.Mvc;

namespace Origami.UI.FrontEnd.Controllers
{
    [Route("[Controller]")]
    public class SitemapController : Controller
    {
        [HttpGet("google")]
        public IActionResult Google()
        {
            Response.ContentType = "application/xml";
            return View();
        }

        [HttpGet("bing")]
        public IActionResult Bing()
        {
            return View();
        }
    }
}
