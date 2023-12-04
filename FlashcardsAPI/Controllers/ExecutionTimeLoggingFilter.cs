using Microsoft.AspNetCore.Mvc;

namespace FlashcardsAPI.Controllers
{
    public class ExecutionTimeLoggingFilter : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
