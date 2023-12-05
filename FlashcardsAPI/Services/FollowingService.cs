
using FlashcardsAPI.Migrations;
using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
	public class FollowingService : IFollowingService
	{
		private readonly ApplicationDbContext _context;

		public FollowingService(ApplicationDbContext context) 
		{
			_context = context;
		}
		public async Task<bool> AddFollowing(Following following)
		{
			if (_context.Followings == null)
			{
				return false;
			}
			_context.Followings.Add(following);
			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException)
			{
				throw;
			}

			return true;
		}

		public async Task<bool> DeleteFollowing(string followingId, string followedId)
		{
			if (_context.Followings == null)
			{
				return false;
			}
			var following = await _context.Followings.FirstAsync(f => f.FollowingUserId == followingId && f.FollowedUserId == followedId);
			if (following == null)
			{
				return false;
			}

			_context.Followings.Remove(following);
			await _context.SaveChangesAsync();

			return true;

		}

        public async Task<List<FlashcardCollection<Flashcards>>?> GetFollowingFlashcardCollections(string id)
        {
            if (_context.Followings == null || _context.FlashcardCollection == null)
            {
                return null;
            }

            var followings = await _context.Followings.Where(f => f.FollowingUserId == id).ToListAsync();

            List<string?> followingIds = new();

			foreach (var f in followings)
			{
				followingIds.Add(f.FollowedUserId);
			}

			List<FlashcardCollection<Flashcards>>? collecitons = await _context.FlashcardCollection.Where(c => followingIds.Contains(c.FlashcardsAppUserId)).ToListAsync();

			return collecitons;
        }

        public async Task<List<Following>?> GetUsersFollowedByUserWithId(string id)
		{
			if (_context.Followings == null)
			{
				return null;
			}
			var following = await _context.Followings.Where(f => f.FollowingUserId == id).ToListAsync();

			if (following == null)
			{
				return null;
			}

			return following;
		}

		public async Task<List<Following>?> GetUsersThatFollowUserWithId(string id)
		{
			if (_context.Followings == null)
			{
				return null;
			}
			var following = await _context.Followings.Where(f => f.FollowedUserId == id).ToListAsync();

			if (following == null)
			{
				return null;
			}

			return following;
		}
	}
}
