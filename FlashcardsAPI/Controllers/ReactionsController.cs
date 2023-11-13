using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionsController : Controller
    {
        private readonly IFlashcardsAppDbService _flashcardsAppDbService;

        public ReactionsController(IFlashcardsAppDbService service)
        {
            _flashcardsAppDbService = service;
        }
        [HttpPost("ToggleReaction")]
        public async Task<IActionResult> ToggleReaction(int collectionId, ReactionType reactionType, string userId)
        {
            if (string.IsNullOrEmpty(userId) || collectionId <= 0)
            {
               return BadRequest("Invalid parameters.");
            }

           var reaction = await _flashcardsAppDbService.ToggleReaction(collectionId, reactionType, userId);
          

            var reactionCounts = await _flashcardsAppDbService.CalculateReactionCounts(collectionId);
            return Ok(new { message = "Reaction toggled successfully.", reactionCounts });
        }
    }
}

