using FlashcardsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService service)
        {
            _profileService = service;
        }

        // GET api/Profile/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var userProfile = await _profileService.GetProfileAsync(userId);
            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        // POST: api/Profile/EditDescription
        [HttpPost("EditDescription")]
        public async Task<IActionResult> EditDescription(string userId, [FromBody] string description)
        {
            var result = await _profileService.UpdateDescriptionAsync(userId, description);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to update description.");
        }

        [HttpPost("UploadProfilePhoto")]
        public async Task<IActionResult> UploadProfilePhoto(string userId, string profilePhotoPath)
        {
            if (string.IsNullOrEmpty(profilePhotoPath))
            {
                return BadRequest("No file path provided.");
            }

            var updateResult = await _profileService.UpdateProfilePhotoPathAsync(userId, profilePhotoPath);
            if (updateResult)
            {
                return Ok();
            }

            return BadRequest("Error updating the photo path.");
        }
    }
}
