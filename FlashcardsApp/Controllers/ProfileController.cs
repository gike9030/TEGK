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

    }
}