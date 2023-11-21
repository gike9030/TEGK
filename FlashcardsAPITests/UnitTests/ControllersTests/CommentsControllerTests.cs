using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardsAPI.Controllers;
using FlashcardsAPI.Migrations;
using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FlashcardsAPITests.Controllers
{
	[TestClass]
	public class CommentsControllerTests
	{
		[TestMethod]
		public async Task PostComment_CommentValid_ReturnsComment()
		{
			int id = 1;
			Comment comment = new() { Id = id, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.AddComment(comment)).ReturnsAsync(comment);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.PostComment(comment) as CreatedAtActionResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PostComment_FaildToUpload_ReturnsNull()
		{
			int id = 1;
			Comment comment = new() { Id = id, Content = "_uck _e in the a__ tonight" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.AddComment(comment)).ReturnsAsync((Comment?)null);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.PostComment(comment) as StatusCodeResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task GetComment_CommentExsists_ReturnsComment()
		{
			int id = 1;
			Comment comment = new Comment() { Id = id, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.GetComment(id)).ReturnsAsync(comment);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.GetComment(id) as OkObjectResult;

			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Value);
			Assert.IsTrue(((Comment)result.Value).Id == id);
		}

		[TestMethod]
		public async Task GetComment_CommentDoesntExsists_ReturnsNotFound()
		{
			int id = 1;
			Comment comment = new() { Id = id, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.GetComment(id)).ReturnsAsync((Comment?) null);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.GetComment(id) as NotFoundObjectResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutComment_CommentValid_ReturnsNoContent()
		{
			int id = 1;
			Comment comment = new() { Id = id, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.UpdateComment(id, comment)).ReturnsAsync(comment);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.PutComment(id, comment) as NoContentResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutComment_MismatchId_ReturnsBadRequest()
		{
			int id = 1;
			Comment comment = new() { Id = id + 1, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.UpdateComment(id, comment)).ReturnsAsync(comment);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.PutComment(id, comment) as BadRequestObjectResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task PutComment_DatabaseFailsToUpdate_ReturnsNotFound()
		{
			int id = 1;
			Comment comment = new() { Id = id, Content = "IM TEEEEESTINGGGGGG UUHHHHHHHHHHHHH" };
			var mockDb = new Mock<ICommentService>();

			mockDb.Setup(db => db.UpdateComment(id, comment)).ReturnsAsync((Comment?)null);

			var controller = new CommentsController(mockDb.Object);

			var result = await controller.PutComment(id, comment) as NotFoundResult;

			Assert.IsNotNull(result);
		}
	}
}
