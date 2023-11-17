using Moq;
using FlashcardsAPI.Services;
using FlashcardsAPI.Models;
using FlashcardsAPI.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Humanizer.Localisation.TimeToClockNotation;
using System.Reflection.Metadata;

namespace FlashcardsAPITests.Controllers
{
	[TestClass]
	public class FlashcardCollectionsControllerTests
	{
		[TestMethod]
		public async Task GetFlashcardsInCollection_CardsExsists_ShouldReturnCards()
		{
			int collectionId = 1;
			Flashcards flashcard1 = new() { Id = 45, FlashcardCollectionId = 1 };
			Flashcards flashcard2 = new() { Id = 46, FlashcardCollectionId = 1 };
			List<Flashcards> list1 = new()
			{
				flashcard1,
			};
			List<Flashcards> list2 = new()
			{
				flashcard2
			};

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardsInCollection(collectionId)).ReturnsAsync(list1);

			mockFlashcardManager.Setup(fm => fm.GetFlashcardsInCollection(collectionId)).Returns(list2);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardsInCollection(collectionId) as OkObjectResult;

			Assert.IsNotNull(result);
			IEnumerable<Flashcards> flashcardList = (IEnumerable<Flashcards>) result.Value;
			Assert.AreEqual(list1.Count + list2.Count, flashcardList.Count());
		}

		[TestMethod]
		public async Task GetFlashcardsInCollection_CardsDontExsists_ShouldReturnEmptyList()
		{
			int collectionId = 1;
			List<Flashcards> list1 = new();

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardsInCollection(collectionId)).ReturnsAsync(list1);

			mockFlashcardManager.Setup(fm => fm.GetFlashcardsInCollection(collectionId)).Returns(list1);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardsInCollection(collectionId) as OkObjectResult;

			Assert.IsNotNull(result);
			IEnumerable<Flashcards> flashcardList = (IEnumerable<Flashcards>)result.Value;
			Assert.AreEqual(0, flashcardList.Count());
		}

		[TestMethod]
		public async Task GetFlashcardCollections_CollectionsExist_ReturnsCollections()
		{
			List<FlashcardCollection<Flashcards>> flashcardCollectionList = new()
			{
				new FlashcardCollection<Flashcards>()
				{
					Id = 1,
				},
				new FlashcardCollection<Flashcards>
				{
					Id = 2,
				},
				new FlashcardCollection<Flashcards>()
				{
					Id = 3,
				}
			};

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardCollections()).ReturnsAsync(flashcardCollectionList);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardCollections() as OkObjectResult;
			var collections = (IEnumerable<FlashcardCollection<Flashcards>>?)result.Value;

			Assert.IsNotNull(result);
			Assert.IsNotNull(collections);
			//Assert.IsTrue(collections.Any());
			//Assert.IsTrue(collections.First().Id == flashcardCollectionList.First().Id);

		}

		[TestMethod]
		public async Task GetFlashcardCollections_CollectionsDontExist_ReturnsEmptyList()
		{
			List<FlashcardCollection<Flashcards>> flashcardCollectionList = new() { };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardCollections()).ReturnsAsync(flashcardCollectionList);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardCollections() as OkObjectResult;
			var collections = (IEnumerable<FlashcardCollection<Flashcards>>?)result.Value;

			Assert.IsNotNull(result);
			Assert.IsFalse(collections.Any());
		}

		[TestMethod]
		public async Task GetFlashcardCollections_ServerError_ReturnsBadRequest()
		{
			List<FlashcardCollection<Flashcards>> flashcardCollectionList = new() { };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardCollections()).ReturnsAsync((List<FlashcardCollection<Flashcards>>?)null);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardCollections() as BadRequestResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task GetFlashcardCollections_IdValid_ReturnsCollection()
		{
			int id = 1;
			FlashcardCollection<Flashcards> flashcardCollection = new() { Id = id };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardCollection(id)).ReturnsAsync(flashcardCollection);

			mockFlashcardManager.Setup(fm => fm.GetFlashcardsInCollection(id)).Returns(new List<Flashcards>());

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardCollections(id);
			var collection = result.Value;

			Assert.IsNotNull(result);
			Assert.IsNotNull(collection);
			Assert.AreEqual(id, collection.Id);

		}

		[TestMethod]
		public async Task GetFlashcardCollections_IdInvalid_ReturnsError()
		{
			int id = 1;
			FlashcardCollection<Flashcards> flashcardCollection = new() { Id = id };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcardCollection(id)).ReturnsAsync(flashcardCollection);

			mockFlashcardManager.Setup(fm => fm.GetFlashcardsInCollection(id)).Returns(new List<Flashcards>());

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcardCollections(id + 1);
			var collection = result.Value;

			Assert.IsNotNull(result);
			Assert.IsNull(collection);
		}

		[TestMethod]
		public async Task PutFlashcardCollection_CollectionExists_UpdatesCollection()
		{
			FlashcardCollection<Flashcards> collection = new() { Id = 1, CollectionName = "One" };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.UpdateFlashcardCollection(collection.Id, collection)).ReturnsAsync(collection);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcardCollection(collection) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutFlashcardCollection_CollectionDoesntExists_ReturnsBadRequest()
		{
			FlashcardCollection<Flashcards> collection = new() { Id = 1, CollectionName = "One" };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.UpdateFlashcardCollection(collection.Id, collection)).ReturnsAsync((FlashcardCollection<Flashcards>?)null);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcardCollection(collection) as BadRequestResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcardCollections_IdExists_DeletesCollection()
		{
			int id = 1;
			FlashcardCollection<Flashcards> collection = new() { Id = id, CollectionName = "One" };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.DeleteFlashcardCollection(collection.Id)).ReturnsAsync(true);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.DeleteFlashcardCollections(id) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcardCollections_IdDoesntExists_ReturnsBadRequest()
		{
			int id = 1;
			FlashcardCollection<Flashcards> collection = new() { Id = id, CollectionName = "One" };

			var mockDb = new Mock<IFlashcardCollectionService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.DeleteFlashcardCollection(collection.Id)).ReturnsAsync(false);

			var controller = new FlashcardCollectionsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.DeleteFlashcardCollections(id) as BadRequestResult;

			Assert.IsNotNull(result);
		}
	}
}
