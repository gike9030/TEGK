using System.Collections.Concurrent;
using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;

namespace FlashcardsAPI.Services
{
	// TO ADD file to store flashcards in case of system shutdown

	public class FlashcardsStorageService
	{
		private readonly IServiceProvider _serviceProvider;
		private Object _lock = new();

		private ConcurrentDictionary<int, Flashcards> _flashcards = new();
		private Timer _databaseUpdateTimer;

		public FlashcardsStorageService(IServiceProvider serviceProvider) 
		{
			_serviceProvider = serviceProvider;
			_databaseUpdateTimer = new Timer(UpdateDatabase, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
		}
		public void AddFlashcard( Flashcards flashcard)
		{
			_flashcards.TryAdd(flashcard.Id, flashcard);
		}

		public Flashcards? GetFlashcard(int id)
		{
			_flashcards.TryGetValue(id, out var flashcard);
			return flashcard;
		}

		public List<Flashcards> GetFlashcardsInCollection(int? id)
		{
			return _flashcards.Values.Where(flashcard => flashcard.FlashcardCollectionId == id).ToList();
		}

		public void UpdateFlashcard(int id, Flashcards newFlashcard)
		{
			if (GetFlashcard(id) != null)
			{
				_flashcards.TryUpdate(id, newFlashcard, GetFlashcard(id));
			}
		}

		public bool RemoveFlashcard(int id)
		{
			return _flashcards.TryRemove(id, out var _);
		}

		public void RemoveFlashcardsFromCollection(int id)
		{
			foreach(var flashcard in _flashcards.Values)
			{
				if (flashcard.FlashcardCollectionId == id)
				{
					_flashcards.TryRemove(flashcard.Id, out var _);
				}
			}
		}

		private void UpdateDatabase(Object? _)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				foreach (var flashcard in _flashcards.Values)
				{
					flashcard.Id = 0;
					dbContext.Flashcards.Add(flashcard);
				}

				dbContext.SaveChanges();

			}

			_flashcards.Clear();
		}
	}
}
