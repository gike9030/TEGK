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
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(UserManager<FlashcardsAppUser> userManager, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory, ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _environment = environment;
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
            _logger = logger;
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
            _logger.LogInformation("UploadProfilePhoto method called");
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

            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", uniqueFileName);

            try
            {
                var directoryPath = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePhoto.CopyToAsync(stream);
                }

                var profilePhotoPath = $"uploads/{uniqueFileName}";
                await UpdateProfilePhotoPathInApi(userId, profilePhotoPath);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Error saving the photo.");
                return View("Index");
            }
        }

        private async Task<bool> UpdateProfilePhotoPathInApi(string userId, string profilePhotoPath)
        {
            string requestUri = $"api/Profile/UploadProfilePhoto?userId={Uri.EscapeDataString(userId)}&profilePhotoPath={Uri.EscapeDataString(profilePhotoPath)}";

            var response = await _httpClient.PostAsync(requestUri, null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API call failed with status code {response.StatusCode} and message: {await response.Content.ReadAsStringAsync()}");
                return false;
            }

            return true;
        }

    }
}

