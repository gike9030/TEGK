using System;
using System.Linq;
using System.Threading.Tasks;
using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlashcardsAPI.IntegrationTests.Services
{
	[TestClass]
	public class CommentServiceIntegrationTests
	{
		private ApplicationDbContext _dbContext;
		private CommentService _commentService;

		[TestInitialize]
		public void Initialize()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
				.Options;

			_dbContext = new ApplicationDbContext(options);
			_commentService = new CommentService(_dbContext);
		}

		[TestMethod]
		public async Task GetComment_ValidId_ReturnsComment()
		{
			var expectedComment = new Comment { Id = 1, Content = "Test Comment" };
			await _dbContext.Comments.AddAsync(expectedComment);
			await _dbContext.SaveChangesAsync();

			var result = await _commentService.GetComment(expectedComment.Id);

			Assert.IsNotNull(result);
			Assert.AreEqual(expectedComment.Content, result.Content);
		}

		[TestMethod]
		public async Task GetComment_InvalidId_ReturnsNull()
		{
			var result = await _commentService.GetComment(123);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task GetComment_ErrorHandling_ReturnsNull()
		{
			_dbContext.Comments = null;

			var result = await _commentService.GetComment(1);

			Assert.IsNull(result);
		}

		[TestMethod]
		public async Task AddComment_ValidComment_ReturnsComment()
		{
			var validComment = new Comment { Id = 1, Content = "Test Comment" };

			var result = await _commentService.AddComment(validComment);

			Assert.AreEqual(validComment, result);
		}

		[TestMethod]
		public async Task DeleteComment_ValidId_ReturnsTrue()
		{
			var existingComment = new Comment { Id = 1, Content = "Test Comment" };
			await _dbContext.Comments.AddAsync(existingComment);
			await _dbContext.SaveChangesAsync();

			var result = await _commentService.DeleteComment(existingComment.Id);

			Assert.IsTrue(result);
			Assert.IsNull(await _dbContext.Comments.FindAsync(existingComment.Id));
		}
		[TestMethod]
		public async Task UpdateComment_ValidIdAndComment_ReturnsUpdatedComment()
		{
			// Arrange
			var existingComment = new Comment { Id = 3, Content = "Test Comment" };
			await _dbContext.Comments.AddAsync(existingComment);
			await _dbContext.SaveChangesAsync();

			existingComment.Content = "new";
			// Act
			var result = await _commentService.UpdateComment(existingComment.Id, existingComment);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(existingComment.Content, result.Content);
		}

		[TestMethod]
		public async Task UpdateComment_InvalidId_ReturnsNull()
		{
			// Arrange
			var updatedComment = new Comment { Id = 1, Content = "Updated Comment" };

			// Act
			var result = await _commentService.UpdateComment(updatedComment.Id, updatedComment);

			// Assert
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
