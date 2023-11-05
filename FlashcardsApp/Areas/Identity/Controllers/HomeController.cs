using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
