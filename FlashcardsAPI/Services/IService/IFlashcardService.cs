using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IFlashcardService
	{
		Task<Flashcards?> GetFlashcard(int id);
		Task<Flashcards?> AddFlashcard(Flashcards flashcards);
		Task<Flashcards?> UpdateFlashcard(int id, Flashcards flashcards);
		Task<bool> DeleteFlashcard(int id);
	}
}
