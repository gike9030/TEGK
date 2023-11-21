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
	public class ReactionsControllerTests
	{
		[TestMethod]
		public async Task ToggleReaction_ValidRequest_ReturnsOk()
		{
			string userId = "562f-fwe8-v668-eAD7";
			int collectionId = 1;
			ReactionType reactionType = ReactionType.Hearts;
			Reaction<Flashcards> reaction = new() { FlashcardCollectionId = collectionId, UserId = userId, Type = reactionType };

			var mockDb = new Mock<IReactionService>();

			mockDb.Setup(db => db.ToggleReaction(collectionId, reactionType, userId)).ReturnsAsync(reaction);

			mockDb.Setup(db => db.CalculateReactionCounts(collectionId)).ReturnsAsync(new Dictionary<ReactionType, int>());

			var controller = new ReactionsController(mockDb.Object);

			var result = await controller.ToggleReaction(collectionId, reactionType, userId) as OkObjectResult;

			Assert.IsNotNull(result);
		}

		[TestMethod]
		public async Task ToggleReaction_BadRequest_ReturnsBadRequest()
		{
			string userId = "562f-fwe8-v668-eAD7";
			int collectionId = -5;
			ReactionType reactionType = ReactionType.Hearts;
			Reaction<Flashcards> reaction = new() { FlashcardCollectionId = collectionId, UserId = userId, Type = reactionType };

			var mockDb = new Mock<IReactionService>();

			mockDb.Setup(db => db.ToggleReaction(collectionId, reactionType, userId)).ReturnsAsync(reaction);

			mockDb.Setup(db => db.CalculateReactionCounts(collectionId)).ReturnsAsync(new Dictionary<ReactionType, int>());

			var controller = new ReactionsController(mockDb.Object);

			var result = await controller.ToggleReaction(collectionId, reactionType, userId) as BadRequestObjectResult;

			Assert.IsNotNull(result);
		}
	}
}
