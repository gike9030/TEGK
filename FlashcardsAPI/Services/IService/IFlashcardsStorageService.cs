using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IFlashcardsStorageService
	{
		public void RemoveFlashcardsFromCollection(int id);
		public bool RemoveFlashcard(int id);
		public void UpdateFlashcard(int id, Flashcards newFlashcard);
		public List<Flashcards> GetFlashcardsInCollection(int? id);
		public Flashcards? GetFlashcard(int id);
		public void AddFlashcard(Flashcards flashcard);

	}
}
