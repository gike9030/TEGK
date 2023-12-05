using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IFollowingService
	{
		Task<List<Following>?> GetUsersThatFollowUserWithId(string id);
		Task<List<Following>?> GetUsersFollowedByUserWithId(string id);
		Task<bool> DeleteFollowing(string followingId, string followedId);
		Task<bool> AddFollowing(Following following);
		Task<List<FlashcardCollection<Flashcards>>?> GetFollowingFlashcardCollections(string id);
	}
}
