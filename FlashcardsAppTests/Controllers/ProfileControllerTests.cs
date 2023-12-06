using FlashcardsApp.Controllers;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;
using Moq.Protected;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FlashcardsAppTests.Controllers
{
    [TestClass]
    public class ProfileControllerTests
    {
        [TestMethod]
        public async Task Index_Success_ReturnsProfileView()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var profile = new Profile { };
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var controller = new ProfileController(mockUserManager.Object, null, mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);

            var model = result.Model as Profile;
            Assert.IsNotNull(model);
            Assert.AreEqual(profile.FirstName, model.FirstName);
            Assert.AreEqual(profile.LastName, model.LastName);
            Assert.AreEqual(profile.ProfilePhoto, model.ProfilePhoto);
            Assert.AreEqual(profile.Description, model.Description);
            Assert.AreEqual(profile.FlashcardCollections, model.FlashcardCollections);
            Assert.AreEqual(profile.ProfilePhotoUpload, model.ProfilePhotoUpload);
        }

        [TestMethod]
        public async Task Index_Failure_ReturnsErrorView()
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

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var controller = new ProfileController(mockUserManager.Object, null, mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task EditDescription_Success_RedirectsToIndex()
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

            var mockUserManager = UserManagerMock.CreateMockUserManager();
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var controller = new ProfileController(mockUserManager.Object, null, mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.EditDescription("New Description") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task EditDescription_Failure_ReturnsErrorView()
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
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var controller = new ProfileController(mockUserManager.Object, null, mockHttpClientFactory.Object, null);

            // Act
            var result = await controller.EditDescription(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);
        }

        [TestMethod]
        public async Task UploadProfilePhoto_Success_RedirectsToIndex()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var mockLogger = new Mock<ILogger<ProfileController>>();

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
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns("testPath");

            var profilePhoto = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image data")), 0, 100, "Data", "test.jpg");

            var controller = new ProfileController(mockUserManager.Object, mockEnvironment.Object, mockHttpClientFactory.Object, mockLogger.Object);

            // Act
            var result = await controller.UploadProfilePhoto(profilePhoto) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task UploadProfilePhoto_FileNull_ReturnsIndexWithError()
        {
            // Arrange
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var mockLogger = new Mock<ILogger<ProfileController>>();

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
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns("testPath");

            var controller = new ProfileController(mockUserManager.Object, mockEnvironment.Object, mockHttpClientFactory.Object, mockLogger.Object);

            // Act
            var result = await controller.UploadProfilePhoto(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
        }

        [TestMethod]
        public async Task UploadProfilePhoto_InvalidExtension_ReturnsIndexWithError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var mockLogger = new Mock<ILogger<ProfileController>>();

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
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns("testPath");

            var invalidFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image data")), 0, 100, "Data", "test.png");

            var controller = new ProfileController(mockUserManager.Object, mockEnvironment.Object, mockHttpClientFactory.Object, mockLogger.Object);

            // Act
            var result = await controller.UploadProfilePhoto(invalidFile) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
        }

        [TestMethod]
        public async Task UploadProfilePhoto_ExceedsSizeLimit_ReturnsIndexWithError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var mockLogger = new Mock<ILogger<ProfileController>>();

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
            mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns("UserId");

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(e => e.WebRootPath).Returns("testPath");

            var largeFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes(new string('a', 6 * 1024 * 1024))), 0, 6 * 1024 * 1024, "Data", "test.jpg");

            var controller = new ProfileController(mockUserManager.Object, mockEnvironment.Object, mockHttpClientFactory.Object, mockLogger.Object);

            // Act
            var result = await controller.UploadProfilePhoto(largeFile) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
            Assert.IsTrue(controller.ModelState.ContainsKey(string.Empty));
        }
    }
}
