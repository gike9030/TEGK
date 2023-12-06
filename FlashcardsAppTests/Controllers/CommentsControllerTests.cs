using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FlashcardsApp.Models;
using FlashcardsApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using FlashcardsApp.Services;
using FlashcardsApp.Areas.Identity.Data;

namespace FlashcardsAppTests.Controllers
{
    [TestClass]
    public class CommentsControllerTests
    {
        [TestMethod]
        public void TestCreateViewResult()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUserManager = UserManagerMock.CreateMockUserManager();

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName));
        }

        [TestMethod]
        public async Task Create_Success_RedirectsToIndex()
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

            var mockUserManager = UserManagerMock.CreateMockUserManager();

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.Create(1, "Test Content") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("FlashcardCollection", result.ControllerName);
        }

        [TestMethod]
        public async Task Create_UserNotFound_ReturnsErrorView()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                           .ReturnsAsync(value: null);// Simulate user not found

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.Create(1, "Test Content") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task Create_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockUserManager = UserManagerMock.CreateMockUserManager();

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);
            controller.ModelState.AddModelError("TestError", "Test error message");

            // Act
            var result = await controller.Create(1, "Test Content") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public async Task Create_ApiFailureWithBadRequest_ReturnsViewWithModelAndError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var mockUserManager = UserManagerMock.CreateMockUserManager();

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.Create(1, "Test Content") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull(result.Model);
            Assert.AreEqual("Test Content", ((Comment)result.Model).Content);
            Assert.IsTrue(controller.ModelState[string.Empty].Errors.Any());
        }

        [TestMethod]
        public async Task Create_ApiFailure_ReturnsErrorViewWithModel()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var mockUserManager = UserManagerMock.CreateMockUserManager();

            var controller = new CommentsController(mockHttpClientFactory.Object, mockUserManager.Object);

            // Act
            var result = await controller.Create(1, "Test Content") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
            Assert.IsNotNull(result.Model);
            Assert.AreEqual("Test Content", ((Comment)result.Model).Content);
        }

        [TestMethod]
        public async Task Details_Success_ReturnsViewWithModel()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new Comment { }), Encoding.UTF8, "application/json")
            };

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(Comment));
        }

        [TestMethod]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Details(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


        [TestMethod]
        public async Task Details_ApiRequestFails_ReturnsNotFound()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Details(1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_Success_ReturnsViewWithModel()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new Comment { }), Encoding.UTF8, "application/json")
            };

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(Comment));
        }

        [TestMethod]
        public async Task Edit_NullId_ReturnsNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Edit(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_ApiRequestFails_ReturnsNotFound()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Edit(1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_Success_RedirectsToIndex()
        {
            // Arrange
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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);
            var comment = new Comment { Id = 1, Content = "Test Content", FlashcardCollectionId = 123, UserId = "User123" };

            // Act
            var result = await controller.Edit(1, comment) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task Edit_IdMismatch_ReturnsNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new CommentsController(mockHttpClientFactory.Object, null);
            var comment = new Comment { Id = 2, Content = "Test Content", FlashcardCollectionId = 123, UserId = "User123" };

            // Act
            var result = await controller.Edit(1, comment) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Edit_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new CommentsController(mockHttpClientFactory.Object, null);
            controller.ModelState.AddModelError("TestError", "Test error message");
            var comment = new Comment { Id = 1, Content = "Test Content", FlashcardCollectionId = 123, UserId = "User123" };

            // Act
            var result = await controller.Edit(1, comment) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public async Task Edit_ApiRequestFails_ReturnsViewWithModelAndError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);
            var comment = new Comment { Id = 1, Content = "Test Content", FlashcardCollectionId = 123, UserId = "User123" };

            // Act
            var result = await controller.Edit(1, comment) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
            Assert.AreEqual("Failed to update the comment. Please try again later.", controller.ModelState[string.Empty].Errors.First().ErrorMessage);
            Assert.AreEqual(comment, result.Model);
        }

        [TestMethod]
        public async Task Delete_Success_ReturnsViewWithModel()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new Comment { }), Encoding.UTF8, "application/json")
            };

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Delete(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(Comment));
        }

        [TestMethod]
        public async Task Delete_NullId_ReturnsNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Delete(null) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Delete_ApiRequestFails_ReturnsNotFound()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Delete(1) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteConfirmed_Success_RedirectsToIndex()
        {
            // Arrange
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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.DeleteConfirmed(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ApiRequestFails_ReturnsDeleteViewWithModelError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new CommentsController(mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.DeleteConfirmed(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Delete", result.ViewName);
            Assert.IsTrue(controller.ModelState[string.Empty].Errors.Count > 0);
            Assert.AreEqual("Failed to delete the comment. Please try again later.", controller.ModelState[string.Empty].Errors.First().ErrorMessage);
        }


    }
}
