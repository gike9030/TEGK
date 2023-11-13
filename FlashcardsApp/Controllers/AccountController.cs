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

        public ProfileController(UserManager<FlashcardsAppUser> userManager, FlashcardsAppContext context)
        {
            _userManager = userManager;
            _context = context;
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

            // Update the user's description
            user.Description = description;

            // Save the changes in the database
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                // Redirect back to the profile page or wherever appropriate
                return RedirectToAction(nameof(Index));
            }

            // Handle errors here (e.g., log them and return an error view)
            return View("Error");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var collection = _context.FlashcardCollection.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var collection = _context.FlashcardCollection.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            _context.FlashcardCollection.Remove(collection);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ViewCollection(int id)
        {
            var collection = _context.FlashcardCollection.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View();
        }
        [HttpGet]
        public IActionResult PlayCollection(int id)
        {
            var collection = _context.FlashcardCollection.FirstOrDefault(c => c.Id == id);
            if (collection == null)
            {
                return NotFound();
            }

            return View();
        }


    }
}

