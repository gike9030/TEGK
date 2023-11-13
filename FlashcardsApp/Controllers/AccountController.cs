using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.Models;
using System.Threading.Tasks;
using System.Linq;
using FlashcardsApp.Data;

namespace FlashcardsApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<FlashcardsAppUser> _userManager;
        private readonly FlashcardsAppContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(UserManager<FlashcardsAppUser> userManager, FlashcardsAppContext context, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _context = context;
            _environment = environment;
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new Profile
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePhoto = user.ProfilePhotoPath,
                Description = user.Description,
                FlashcardCollections = _context.FlashcardCollection
                                               .Where(c => c.FlashcardsAppUserId == userId)
                                               .ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditDescription(string description)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Description = description;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {

                return RedirectToAction(nameof(Index));
            }


            return View("Error");
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile profilePhoto)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                
                var extension = Path.GetExtension(profilePhoto.FileName).ToLower();
                if (extension != ".jpg")
                {
                    ModelState.AddModelError(string.Empty, "Only .jpg files are allowed.");
                    return View("Index", user);
                }

                const int maxFileSize = 5 * 1024 * 1024;
                if (profilePhoto.Length > maxFileSize)
                {
                    ModelState.AddModelError(string.Empty, "File size cannot exceed 5 MB.");
                    return View("Index", user);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{profilePhoto.FileName}";
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", uniqueFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = System.IO.File.Create(filePath))
                {
                    await profilePhoto.CopyToAsync(stream);
                }

                user.ProfilePhotoPath = Path.Combine("uploads", uniqueFileName);
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error uploading the photo.");
                }
            }

            return View("Index", user);
        }



    }
}

