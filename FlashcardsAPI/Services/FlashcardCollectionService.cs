using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
	public class FlashcardCollectionService : IFlashcardCollectionService
	{
		private readonly ApplicationDbContext _context;
		public FlashcardCollectionService(ApplicationDbContext context)
		{
			_context = context;
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

		public async Task<bool> DeleteFlashcardCollection(int id)
		{
			try
			{
				FlashcardCollection<Flashcards>? collection = await _context.FlashcardCollection
				.Include(c => c.Flashcards)
				.Include(c => c.Comments)
                .Include(c => c.Reactions)
                .FirstOrDefaultAsync(c => c.Id == id);

				foreach (var flashcard in collection.Flashcards)
				{
					_context.Flashcards.Remove(flashcard);
				}

				foreach (var comment in collection.Comments)
				{
					_context.Comments.Remove(comment);
				}
                foreach (var reaction in collection.Reactions)
                {
                    _context.Reactions.Remove(reaction);
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
				.Include(collection => collection.Reactions)
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
