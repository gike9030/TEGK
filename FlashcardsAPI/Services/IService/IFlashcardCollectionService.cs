﻿using FlashcardsAPI.Models;

namespace FlashcardsAPI.Services
{
	public interface IFlashcardCollectionService
	{
		Task<FlashcardCollection<Flashcards>?> GetFlashcardCollection(int id);
		Task<FlashcardCollection<Flashcards>?> AddFlashcardsCollection(FlashcardCollection<Flashcards> collection);
		Task<FlashcardCollection<Flashcards>?> UpdateFlashcardCollection(int id, FlashcardCollection<Flashcards> collection);
		Task<bool> DeleteFlashcardCollection(int id);
		Task<IEnumerable<Flashcards>?> GetFlashcardsInCollection(int id);
		Task<IEnumerable<FlashcardCollection<Flashcards>>?> GetFlashcardCollections();
		Task<IEnumerable<FlashcardCollection<Flashcards>>?> GetFlashcardCollectionsByCategory(Category category);

	}
}
