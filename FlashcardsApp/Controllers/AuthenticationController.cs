using FlashcardsApp.Areas.Identity.Pages.Account;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace FlashcardsApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;

        public AuthenticationController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        // UNSAFE SENDS UNENCRYPTED PASSWORD
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = HttpApiService.PostToAPI(_httpClient, "/Authenticate/login", model);
                TokenDataModel? tokenDataModel = ObjectSerialiser.Deserialise<TokenDataModel>(res);

                if (!res.IsSuccessStatusCode || tokenDataModel == null || tokenDataModel.Token == null || tokenDataModel.Id == null)
                {
                    // Redirect to Login Identity page
                    // Update temp data for error
                    return RedirectToAction("Index", "Home");
                }

                Response.Cookies.Append("token", tokenDataModel.Token, new CookieOptions() { HttpOnly = true });

                Response.Cookies.Append("id", tokenDataModel.Id);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            if (Request.Cookies["token"] != null && Request.Cookies["id"] != null)
            {
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("id");
            }

            // Redirect to home
            return RedirectToAction("Index", "Home");
        }

        // UNSAFE SENDS UNENCRYPTED PASSWORD
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage res = HttpApiService.PostToAPI(_httpClient, "/Authenticate/Register", new { model.FirstName, model.LastName, Username = model.Email, model.Email, model.Password});

                if (!res.IsSuccessStatusCode)
                {
                    // Redirect to registration
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Login", new {Username = model.Email, model.Password});
            }

            // Redirect to register
            return RedirectToAction("Index", "Home");
        }
    }
}
