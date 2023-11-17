using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
	public class ReactionService : IReactionService
	{
		private readonly ApplicationDbContext _context;
		public ReactionService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Reaction<Flashcards>> ToggleReaction(int collectionId, ReactionType reactionType, string userId)
		{
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
			return existingReaction;
		}

		public async Task<Dictionary<ReactionType, int>> CalculateReactionCounts(int collectionId)
		{
			return await _context.Reactions
				.Where(r => r.FlashcardCollectionId == collectionId)
				.GroupBy(r => r.Type)
				.ToDictionaryAsync(g => g.Key, g => g.Count());
		}
	}
}
