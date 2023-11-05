using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.ViewModels;
using System.Threading.Tasks;

namespace FlashcardsApp.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<FlashcardsAppUser> _userManager;

        public AccountController(UserManager<FlashcardsAppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(model);
        }
    }
}
