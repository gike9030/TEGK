using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Services
{
	public class FlashcardService : IFlashcardService
	{
		private readonly ApplicationDbContext _context;
		public FlashcardService(ApplicationDbContext context)
		{
			_context = context;
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

		public async Task<Flashcards?> GetFlashcard(int id)
		{
			if (_context.Flashcards == null)
			{
				return null;
			}

			var flashcard = await _context.Flashcards.FindAsync(id);

			return flashcard;
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

	}
}
