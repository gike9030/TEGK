using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    public class FlashcardCollectionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
