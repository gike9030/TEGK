using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IReactionService
	{
		Task<Reaction<Flashcards>?> GetReaction(int id);
		Task<Reaction<Flashcards>?> AddReaction(Reaction<Flashcards> reaction);
		Task<Reaction<Flashcards>?> UpdateReaction(int id, Reaction<Flashcards> reaction);
		Task<bool> DeleteReaction(int id);
	}
}
