using System;
using System.Threading.Tasks;
using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FlashcardsAPI.Tests.Services
{
	[TestClass]
	public class FlashcardServiceTests
	{
		private ApplicationDbContext _dbContext;
		private FlashcardService _flashcardService;

		[TestInitialize]
		public void Initialize()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
				.Options;

			_dbContext = new ApplicationDbContext(options);
			_flashcardService = new FlashcardService(_dbContext);
		}

		[TestMethod]
		public async Task AddFlashcard_ValidFlashcard_ReturnsFlashcard()
		{
			var flashcard = new Flashcards { Question = "Question 1", Answer = "Answer 1" };

			var result = await _flashcardService.AddFlashcard(flashcard);

			Assert.IsNotNull(result);
			Assert.AreEqual(flashcard.Question, result.Question);
			Assert.AreEqual(flashcard.Answer, result.Answer);
		}

		[TestMethod]
		public async Task AddFlashcard_DbContextIsNull_ReturnsNull()
		{
			var flashcard = new Flashcards { Question = "Question 1", Answer = "Answer 1" };

			_dbContext.Flashcards = null;

			var result = await _flashcardService.AddFlashcard(flashcard);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcard_ValidId_ReturnsTrue()
		{
			var flashcardId = 1;
			var flashcard = new Flashcards {  Question = "Question 1", Answer = "Answer 1" };
			
			await _dbContext.AddAsync(flashcard);

			var result = await _flashcardService.DeleteFlashcard(flashcardId);

			Assert.IsTrue(result);
		}

		[TestMethod]
		public async Task DeleteFlashcard_InvalidId_ReturnsFalse()
		{
			var flashcardId = 1;

			var result = await _flashcardService.DeleteFlashcard(flashcardId);

			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task GetFlashcard_ValidId_ReturnsFlashcard()
		{
			var flashcardId = 1;
			var flashcard = new Flashcards { Id = flashcardId, Question = "Question 1", Answer = "Answer 1" };
			await _dbContext.AddAsync(flashcard);
			await _dbContext.SaveChangesAsync();

			var result = await _flashcardService.GetFlashcard(flashcardId);

			Assert.IsNotNull(result);
			Assert.AreEqual(flashcard.Id, result.Id);
			Assert.AreEqual(flashcard.Question, result.Question);
			Assert.AreEqual(flashcard.Answer, result.Answer);
		}

		[TestMethod]
		public async Task GetFlashcard_InvalidId_ReturnsNull()
		{
			var flashcardId = 1;

			var result = await _flashcardService.GetFlashcard(flashcardId);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task UpdateFlashcard_ValidId_ReturnsUpdatedFlashcard()
		{
			var originalFlashcard = new Flashcards { Question = "Question 1", Answer = "Answer 1" };
			await _dbContext.AddAsync(originalFlashcard);
			await _dbContext.SaveChangesAsync();

			originalFlashcard.Answer = "HHWFHH";

			var result = await _flashcardService.UpdateFlashcard(originalFlashcard.Id, originalFlashcard);

			Assert.IsNotNull(result);
			Assert.AreEqual(originalFlashcard.Id, result.Id);
			Assert.AreEqual(originalFlashcard.Question, result.Question);
			Assert.AreEqual(originalFlashcard.Answer, result.Answer);
		}

		[TestMethod]
		public async Task UpdateFlashcard_InvalidId_ReturnsNull()
		{
			var flashcardId = 1;
			var updatedFlashcard = new Flashcards { Id = flashcardId, Question = "Updated Question", Answer = "Updated Answer" };

			var result = await _flashcardService.UpdateFlashcard(flashcardId, updatedFlashcard);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task UpdateFlashcard_DifferentId_ReturnsNull()
		{
			var flashcardId = 1;
			var originalFlashcard = new Flashcards { Id = flashcardId, Question = "Question 1", Answer = "Answer 1" };
			await _dbContext.AddAsync(originalFlashcard);
			await _dbContext.SaveChangesAsync();

			var updatedFlashcard = new Flashcards { Id = 2, Question = "Updated Question", Answer = "Updated Answer" };

			var result = await _flashcardService.UpdateFlashcard(flashcardId, updatedFlashcard);

			Assert.IsNull(result);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_dbContext.Database.EnsureDeleted();
			_dbContext.Dispose();
		}

	}
}
