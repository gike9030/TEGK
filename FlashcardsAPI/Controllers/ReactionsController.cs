using FlashcardsAPI.Models;
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
        private readonly ApplicationDbContext _context;

        public ReactionsController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("ToggleReaction")]
        public async Task<IActionResult> ToggleReaction(int collectionId, ReactionType reactionType, string userId)
        {
            
                if (string.IsNullOrEmpty(userId) || collectionId <= 0)
                {
                    return BadRequest("Invalid parameters.");
                }
                var existingReaction = await _context.Reactions
                    .FirstOrDefaultAsync(r => r.FlashcardCollectionId == collectionId && r.UserId == userId && r.Type == reactionType);

                if (existingReaction != null)
                {
                    _context.Reactions.Remove(existingReaction);
                }
                else
                {
                    var newReaction = new Reaction<Flashcards>
                    {
                        Type = reactionType,
                        FlashcardCollectionId = collectionId,
                        UserId = userId
                    };
                    _context.Reactions.Add(newReaction);
                }

                await _context.SaveChangesAsync();

                var reactionCounts = await _context.Reactions
                    .Where(r => r.FlashcardCollectionId == collectionId)
                    .GroupBy(r => r.Type)
                    .Select(group => new { ReactionType = group.Key, Count = group.Count() })
                    .ToListAsync();

                return Ok(new { message = "Reaction toggled successfully.", reactionCounts });
            
           
        }
    }




    }

