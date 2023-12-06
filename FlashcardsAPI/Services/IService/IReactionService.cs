using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IReactionService
    { 
        Task<Reaction<Flashcards>> ToggleReaction(int collectionId, ReactionType reactionType, string userId);
        Task<Dictionary<ReactionType, int>> CalculateReactionCounts(int collectionId);
    }
}
