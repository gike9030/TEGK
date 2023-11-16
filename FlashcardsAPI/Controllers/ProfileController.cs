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
        private readonly IFlashcardsAppDbService _flashcardsAppDbService;

        public ProfileController(IFlashcardsAppDbService service)
        {
            _flashcardsAppDbService = service;
        }

        // GET api/Profile/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(string userId)
        {
            var userProfile = await _flashcardsAppDbService.GetProfileAsync(userId);
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
            var result = await _flashcardsAppDbService.UpdateDescriptionAsync(userId, description);
            if (result)
            {
                return Ok();
            }

            return BadRequest("Failed to update description.");
        }

        // POST: api/Profile/UploadProfilePhoto
        [HttpPost("UploadProfilePhoto")]
        public async Task<IActionResult> UploadProfilePhoto(string userId, IFormFile profilePhoto)
        {
            if (profilePhoto == null || profilePhoto.Length == 0)
            {
                return BadRequest("No file provided.");
            }

            var updateResult = await _flashcardsAppDbService.UpdateProfilePhotoAsync(userId, profilePhoto);
            if (updateResult)
            {
                return Ok();
            }

            return BadRequest("Error uploading the photo.");
        }
    }
}
