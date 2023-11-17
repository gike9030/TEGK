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
        private readonly IReactionService _ReactionService;

        public ReactionsController(IReactionService service)
        {
            _ReactionService = service;
        }
        [HttpPost("ToggleReaction")]
        public async Task<IActionResult> ToggleReaction(int collectionId, ReactionType reactionType, string userId)
        {
            if (string.IsNullOrEmpty(userId) || collectionId <= 0)
            {
               return BadRequest("Invalid parameters.");
            }

           var reaction = await _ReactionService.ToggleReaction(collectionId, reactionType, userId);
          

            var reactionCounts = await _ReactionService.CalculateReactionCounts(collectionId);
            return Ok(new { message = "Reaction toggled successfully.", reactionCounts });
        }
    }
}

