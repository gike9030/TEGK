using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
    // Move each interface to seperate Services in the future. Not implemented do to time constraints
    public class FlashcardsAppDbService : IFlashcardsAppDbService
    {
        private readonly ApplicationDbContext _context;

        public FlashcardsAppDbService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(string userId)
        {
            var user = await _context.Users
                                     .Where(u => u.Id == userId)
                                     .Select(u => new Profile
                                     {
                                         FirstName = u.FirstName,
                                         LastName = u.LastName,
                                         ProfilePhoto = u.ProfilePhotoPath,
                                         Description = u.Description,
                                         FlashcardCollections = _context.FlashcardCollection
                                               .Where(c => c.FlashcardsAppUserId == userId)
                                               .ToList()
                                     })
                                     .FirstOrDefaultAsync();

            return user;
        }

        public async Task<bool> UpdateDescriptionAsync(string userId, string newDescription)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Description = newDescription;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Ideally, you might want to log this exception
                return false;
            }
        }

        public async Task<bool> UpdateProfilePhotoPathAsync(string userId, string profilePhotoPath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.ProfilePhotoPath = profilePhotoPath;

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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
        public async Task<Comment?> AddComment(Comment comment)
		{
			if (_context.Comments == null)
			{
				return null;
			}
			
			try
			{
				_context.Comments.Add(comment);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				return null;
			}

			return comment;
		}

		public async Task<Flashcards?> AddFlashcard(Flashcards flashcard)
		{
			if (_context.Flashcards == null)
			{
				return null;
			}

			try
			{
				_context.Flashcards.Add(flashcard);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				return null;
			}

			return flashcard;
		}

		public async Task<FlashcardCollection<Flashcards>?> AddFlashcardsCollection(FlashcardCollection<Flashcards> collection)
		{
			if (_context.FlashcardCollection == null)
			{
				return null;
			}

			try
			{
				_context.FlashcardCollection.Add(collection);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				return null;
			}

			return collection;
		}

		public async Task<bool> DeleteComment(int id)
		{
			try
			{
				Comment? comment = await _context.Comments.FindAsync(id);

				_context.Comments.Remove(comment);
				await _context.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> DeleteFlashcard(int id)
		{
			try
			{
				Flashcards? flashcard = await _context.Flashcards.FindAsync(id);

				_context.Flashcards.Remove(flashcard);
				await _context.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		public async Task<bool> DeleteFlashcardCollection(int id)
		{
			try
			{
				FlashcardCollection<Flashcards>? collection = await _context.FlashcardCollection
				.Include(c => c.Flashcards)
				.Include(c => c.Reactions)
				.Include(c => c.Comments)
				.FirstOrDefaultAsync(c => c.Id == id);

				foreach (var flashcard in collection.Flashcards)
				{
					_context.Flashcards.Remove(flashcard);
				}

				foreach (var comment in collection.Comments)
				{
					_context.Comments.Remove(comment);
				}

				_context.FlashcardCollection.Remove(collection);
				await _context.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}

		}

		public async Task<IEnumerable<Comment>?> GetAllComments()
		{
			if (_context.Comments == null)
			{
				return null;
			}
			return await _context.Comments.ToListAsync();
		}

		public async Task<Comment?> GetComment(int id)
		{
			if (_context.Comments == null)
			{
				return null;
			}

			var comment = await _context.Comments.FindAsync(id);

			return comment;
		}

		public async Task<Flashcards?> GetFlashcard(int id)
		{
			if (_context.Flashcards == null)
			{
				return null;
			}

			var flashcard = await _context.Flashcards.FindAsync(id);

			return flashcard;
		}

		public async Task<FlashcardCollection<Flashcards>?> GetFlashcardCollection(int id)
		{
			if (_context.FlashcardCollection == null)
			{
				return null;
			}

			var collection = await _context.FlashcardCollection
			    .Include(c => c.Comments)
				.Include(c => c.Reactions)
				.Include(c => c.Flashcards)
				.FirstOrDefaultAsync(c => c.Id == id);

			return collection;
		}

		public async Task<IEnumerable<FlashcardCollection<Flashcards>>?> GetFlashcardCollections()
		{
			if (_context.FlashcardCollection == null)
			{
				return null;
			}

			var collectionsWithComments = await _context.FlashcardCollection
				.Include(collection => collection.Comments)
				.Include (collection => collection.Reactions)
				.ToListAsync();

			return collectionsWithComments;
		}

		public async Task<IEnumerable<FlashcardCollection<Flashcards>>?> GetFlashcardCollectionsByCategory(Category category)
		{
			if (_context.FlashcardCollection == null)
			{
				return null;
			}

			var collections = await _context.FlashcardCollection
				.Include(collection => collection.Comments)
				.Include(c => c.Reactions)
				.Where(c => c.Category == category)
				.ToListAsync();

			return collections;

		}

		public async Task<IEnumerable<Flashcards>?> GetFlashcardsInCollection(int id)
		{
			if (_context.FlashcardCollection == null)
			{
				return null;
			}

			var collection = await _context.FlashcardCollection
				.Include(c => c.Flashcards)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (collection == null)
			{
				return null;
			}

			return collection.Flashcards;
		}

		public async Task<Comment?> UpdateComment(int id, Comment comment)
		{
			if (_context.Comments == null || id != comment.Id)
			{
				return null;
			}

			_context.Entry(comment).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return null;
			}

			return comment;
		}

		public async Task<Flashcards?> UpdateFlashcard(int id, Flashcards flashcards)
		{
			if (_context.Flashcards == null || id != flashcards.Id)
			{
				return null;
			}

			_context.Entry(flashcards).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return null;
			}

			return flashcards;
		}

		public async Task<FlashcardCollection<Flashcards>?> UpdateFlashcardCollection(int id, FlashcardCollection<Flashcards> collection)
		{
			if (_context.FlashcardCollection == null || id != collection.Id)
			{
				return null;
			}

			_context.Entry(collection).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				return null;
			}

			return collection;
		}
	}
}
