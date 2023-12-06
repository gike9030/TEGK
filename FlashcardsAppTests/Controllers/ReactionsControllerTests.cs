using FlashcardsApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace FlashcardsAppTests.Controllers
{
    [TestClass]
    public class ReactionsControllerTests
    {
        [TestMethod]
        public async Task ToggleReaction_Success_ReturnsRedirectToAction()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var userId = "YourUserId";

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                            .Returns(userId);


            var controller = new ReactionsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.ToggleReaction(1, ReactionType.Like) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("FlashcardCollection", result.ControllerName);
        }

        [TestMethod]
        public async Task ToggleReaction_Failure_ReturnsUnauthorizedError()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var userId = string.Empty;

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                            .Returns(userId);

            var controller = new ReactionsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.ToggleReaction(1, ReactionType.Like) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public async Task ToggleReaction_Failure_ReturnsRedirectToActionWithError()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            mockResponse.Content = new StringContent("Error Content");

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var userId = "YourUserId";

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                            .Returns(userId);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var controller = new ReactionsController(mockHttpClientFactory.Object, mockUserManager.Object)
            {
                TempData = tempData
            };

            // Act
            var result = await controller.ToggleReaction(1, ReactionType.Like) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ActionName);
            Assert.AreEqual("Failed to toggle reaction. Status code: BadRequest, Content: Error Content", controller.TempData["Error"]);
        }
    }
}
