using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.Models;
using System.Threading.Tasks;
using System.Linq;
using FlashcardsApp.Data;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FlashcardsApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<FlashcardsAppUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient _httpClient;

        public ProfileController(UserManager<FlashcardsAppUser> userManager, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _environment = environment;
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
        }



        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"api/Profile/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var profile = JsonConvert.DeserializeObject<Profile>(content);

                return View(profile);
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> EditDescription(string description)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            // Changed to send raw JSON string
            var jsonString = JsonConvert.SerializeObject(description);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/Profile/EditDescription?userId={userId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile profilePhoto)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId) || profilePhoto == null || profilePhoto.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid request.");
                return View("Index");
            }

            var extension = Path.GetExtension(profilePhoto.FileName).ToLower();
            if (extension != ".jpg")
            {
                ModelState.AddModelError(string.Empty, "Only .jpg files are allowed.");
                return View("Index");
            }

            const int maxFileSize = 5 * 1024 * 1024;
            if (profilePhoto.Length > maxFileSize)
            {
                ModelState.AddModelError(string.Empty, "File size cannot exceed 5 MB.");
                return View("Index");
            }

            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(profilePhoto.OpenReadStream())
            {
                Headers =
        {
            ContentLength = profilePhoto.Length,
            ContentType = new MediaTypeHeaderValue(profilePhoto.ContentType)
        }
            };

            content.Add(fileContent, "profilePhoto", profilePhoto.FileName);
            var response = await _httpClient.PostAsync($"api/Profile/UploadProfilePhoto?userId={userId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Error uploading the photo.");
            return View("Index");
        }



    }
}

