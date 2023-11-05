using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.ViewModels;
using FlashcardsApp.Authorization;
using FlashcardsApp.Services;
using FlashcardsApp.Models;

namespace FlashcardsApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    [AppUserAuthorize]
    public class AccountController : Controller
    {
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;

        public AccountController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        public async Task<IActionResult> UserProfile()
        {
            Response? resFirstName = HttpApiService.GetFromAPI<Response>(_httpClient, "/FlashcardsAppUser/GetFirstName/" + Request.Cookies["id"], token: Request.Cookies["token"]);
            Response? resLastName = HttpApiService.GetFromAPI<Response>(_httpClient, "/FlashcardsAppUser/GetLastName/" + Request.Cookies["id"], token: Request.Cookies["token"]);

            if (resFirstName == null || resLastName == null)
            {
                return NotFound($"Unable to load user with ID '{Request.Cookies["id"]}'.");
            }

            var model = new ProfileViewModel
            {
                FirstName = resFirstName.Message,
                LastName = resLastName.Message,
            };

            return View(model);
        }

		[HttpGet]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("id");
			Response.Cookies.Delete("token");

			// Your custom logout logic here (if needed before the user is logged out)
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogoutConfirmed()
		{
			// Perform your custom logout actions here, if needed

			// Sign the user out using Identity
			Response.Cookies.Delete("id");
			Response.Cookies.Delete("token");

			// Redirect to a custom logout confirmation page or anywhere you prefer
			return RedirectToAction("Index", "Home");
		}

	}
}
