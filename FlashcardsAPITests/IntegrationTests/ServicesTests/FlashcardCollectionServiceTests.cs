using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlashcardsAPI.IntegrationTests.Services
{
	[TestClass]
	public class FlashcardCollectionServiceIntegrationTests
	{
		private ApplicationDbContext _dbContext;
		private FlashcardCollectionService _collectionService;

		[TestInitialize]
		public void Initialize()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
				.Options;

			_dbContext = new ApplicationDbContext(options);
			_collectionService = new FlashcardCollectionService(_dbContext);
		}

		[TestMethod]
		public async Task AddFlashcardsCollection_ValidCollection_ReturnsCollection()
		{
			var validCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};

			var result = await _collectionService.AddFlashcardsCollection(validCollection);

			Assert.IsNotNull(result);
			Assert.AreEqual(validCollection.CollectionName, result.CollectionName);
			Assert.AreEqual(validCollection.Category, result.Category);
		}

		[TestMethod]
		public async Task AddFlashcardsCollection_DbError_ReturnsNull()
		{
			var invalidCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};
			_dbContext.FlashcardCollection = null;

			var result = await _collectionService.AddFlashcardsCollection(invalidCollection);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task DeleteFlashcardCollection_ValidId_ReturnsTrue()
		{
			var existingCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};
			await _dbContext.FlashcardCollection.AddAsync(existingCollection);
			await _dbContext.SaveChangesAsync();

			var result = await _collectionService.DeleteFlashcardCollection(existingCollection.Id);

			Assert.IsTrue(result);
			Assert.IsNull(await _dbContext.FlashcardCollection.FindAsync(existingCollection.Id));
		}

		[TestMethod]
		public async Task DeleteFlashcardCollection_InvalidId_ReturnsFalse()
		{
			var result = await _collectionService.DeleteFlashcardCollection(123);

			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task DeleteFlashcardCollection_ErrorHandling_ReturnsFalse()
		{
			var existingCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			}; 
			await _dbContext.FlashcardCollection.AddAsync(existingCollection);

			_dbContext.FlashcardCollection = null;

			var result = await _collectionService.DeleteFlashcardCollection(existingCollection.Id);

			Assert.IsFalse(result);
		}

		[TestMethod]
		public async Task GetFlashcardCollection_ValidId_ReturnsCollection()
		{
			var expectedCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			}; 

			await _dbContext.FlashcardCollection.AddAsync(expectedCollection);
			await _dbContext.SaveChangesAsync();

			var result = await _collectionService.GetFlashcardCollection(expectedCollection.Id);

			Assert.IsNotNull(result);
			Assert.AreEqual(expectedCollection.CollectionName, result.CollectionName);
			Assert.AreEqual(expectedCollection.Category, result.Category);
		}

		[TestMethod]
		public async Task GetFlashcardCollection_InvalidId_ReturnsNull()
		{
			var result = await _collectionService.GetFlashcardCollection(123);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task GetFlashcardCollection_ErrorHandling_ReturnsNull()
		{
			var existingCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			}; 
			
			await _dbContext.FlashcardCollection.AddAsync(existingCollection);

			_dbContext.FlashcardCollection = null;

			var result = await _collectionService.GetFlashcardCollection(existingCollection.Id);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task GetFlashcardCollections_ReturnsCollections()
		{
			var expectedCollections = new List<FlashcardCollection<Flashcards>>
			{
				new FlashcardCollection<Flashcards> 
				{
				CollectionName = "Collection1",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
				},
				new FlashcardCollection<Flashcards>
				{
				CollectionName = "Collection2",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
				},
			};
			await _dbContext.FlashcardCollection.AddRangeAsync(expectedCollections);
			await _dbContext.SaveChangesAsync();

			var result = await _collectionService.GetFlashcardCollections();

			Assert.IsNotNull(result);
			CollectionAssert.AreEqual(expectedCollections.Select(c => c.CollectionName).ToList(), result.Select(c => c.CollectionName).ToList());
			CollectionAssert.AreEqual(expectedCollections.Select(c => c.Category).ToList(), result.Select(c => c.Category).ToList());
		}

		[TestMethod]
		public async Task GetFlashcardCollections_EmptyCollections_ReturnsEmptyList()
		{
			var result = await _collectionService.GetFlashcardCollections();

			Assert.IsNotNull(result);
			Assert.AreEqual(0, result.Count());
		}

		[TestMethod]
		public async Task GetFlashcardCollections_ErrorHandling_ReturnsNull()
		{
			_dbContext.FlashcardCollection = null;

			var result = await _collectionService.GetFlashcardCollections();

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task UpdateFlashcardCollection_ValidIdAndCollection_ReturnsUpdatedCollection()
		{
			var existingCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};
			
			await _dbContext.FlashcardCollection.AddAsync(existingCollection);
			await _dbContext.SaveChangesAsync();

			existingCollection.CollectionName = "newer collection";
			existingCollection.Category = Category.Mathematics;

			var result = await _collectionService.UpdateFlashcardCollection(existingCollection.Id, existingCollection);

			Assert.IsNotNull(result);
			Assert.AreEqual(existingCollection.CollectionName, result.CollectionName);
			Assert.AreEqual(existingCollection.Category, result.Category);
		}

		[TestMethod]
		public async Task UpdateFlashcardCollection_InvalidId_ReturnsNull()
		{
			var updatedCollection = new FlashcardCollection<Flashcards>
			{
				Id = 5852,
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};

			var result = await _collectionService.UpdateFlashcardCollection(updatedCollection.Id, updatedCollection);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task UpdateFlashcardCollection_ErrorHandling_ReturnsNull()
		{
			var existingCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = "Fewf",
				CreatedDateTime = DateTime.UtcNow,
			};
			await _dbContext.FlashcardCollection.AddAsync(existingCollection);

			_dbContext.FlashcardCollection = null;

			var updatedCollection = new FlashcardCollection<Flashcards>
			{
				CollectionName = "Updated Collection",
				Category = Category.ComputerScience,
				FlashcardsAppUserId = existingCollection.FlashcardsAppUserId,
				CreatedDateTime = DateTime.UtcNow,
			};

			var result = await _collectionService.UpdateFlashcardCollection(existingCollection.Id, updatedCollection);

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
