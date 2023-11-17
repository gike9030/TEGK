using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardsAPI.Controllers;
using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlashcardsAPITests.Controllers
{
	[TestClass]
	public class FlashcardsControllerTests
	{
		[TestMethod]
		public async Task GetFlashcards_FlashcardInDatabase_ReturnsFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcard(id)).ReturnsAsync(flashcard);
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcards(id) as OkObjectResult;

			Assert.IsNotNull(result);

			var ReturnFlashcard = result.Value as Flashcards;

			Assert.IsNotNull(ReturnFlashcard);
			Assert.IsTrue(ReturnFlashcard.Id == id);
		}

		[TestMethod]
		public async Task GetFlashcards_FlashcardInFlashcardsStorage_ReturnsFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcard(id)).ReturnsAsync((Flashcards?)null);
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns(flashcard);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcards(id) as OkObjectResult;

			Assert.IsNotNull(result);

			var ReturnFlashcard = result.Value as Flashcards;

			Assert.IsNotNull(ReturnFlashcard);
			Assert.IsTrue(ReturnFlashcard.Id == id);
		}

		[TestMethod]
		public async Task GetFlashcards_FlashcardDoesntExsist_ReturnsBadRequest()
		{
			int id = 1;
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.GetFlashcard(id)).ReturnsAsync((Flashcards?)null);
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.GetFlashcards(id) as NotFoundResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutFlashcards_FlashcardInFlashcardStorage_UpdatesFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.UpdateFlashcard(id, flashcard)).ReturnsAsync((Flashcards?)null);
			mockFlashcardManager.Setup(fm => fm.UpdateFlashcard(id, flashcard));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns(flashcard);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcards(id, flashcard) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutFlashcards_FlashcardInDatabase_UpdatesFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.UpdateFlashcard(id, flashcard)).ReturnsAsync(flashcard);
			mockFlashcardManager.Setup(fm => fm.UpdateFlashcard(id, flashcard));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcards(id, flashcard) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutFlashcards_FlashcardDoesntExsist_ReturnsBadRequest()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.UpdateFlashcard(id, flashcard)).ReturnsAsync((Flashcards?)null);
			mockFlashcardManager.Setup(fm => fm.UpdateFlashcard(id, flashcard));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcards(id, flashcard) as BadRequestResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PostFlashcards_FlashcardValid_PostsFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockFlashcardManager.Setup(fm => fm.AddFlashcard(flashcard));

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.PutFlashcards(id, flashcard);

			Assert.IsNotNull(result);
			//Assert.IsNotNull(result.Value);
		}

		[TestMethod]
		public async Task DeleteFlashcards_FlashcardInDatabase_DeletesFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.DeleteFlashcard(id)).ReturnsAsync(true);
			mockFlashcardManager.Setup(fm => fm.RemoveFlashcard(id));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.DeleteFlashcards(id) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcards_FlashcardInFlashcardManager_DeletesFlashcard()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.DeleteFlashcard(id)).ReturnsAsync(false);
			mockFlashcardManager.Setup(fm => fm.RemoveFlashcard(id));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns(flashcard);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.DeleteFlashcards(id) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcards_FlashcardDoesntExsist_ReturnsBadRequest()
		{
			int id = 1;
			Flashcards flashcard = new() { Id = id };
			var mockDb = new Mock<IFlashcardService>();
			var mockFlashcardManager = new Mock<IFlashcardsStorageService>();

			mockDb.Setup(db => db.DeleteFlashcard(id)).ReturnsAsync(false);
			mockFlashcardManager.Setup(fm => fm.RemoveFlashcard(id));
			mockFlashcardManager.Setup(fm => fm.GetFlashcard(id)).Returns((Flashcards?)null);

			var controller = new FlashcardsController(mockFlashcardManager.Object, mockDb.Object);

			var result = await controller.DeleteFlashcards(id) as BadRequestResult;

			Assert.IsNotNull(result);
		}
	}
}
