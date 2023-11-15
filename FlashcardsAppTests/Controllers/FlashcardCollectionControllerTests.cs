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

namespace FlashcardsAppTests.Controllers
{
    [TestClass]
    public class FlashcardCollectionControllerTests
    {
        [TestMethod]
        public void TestFetchCollectionsSuccess()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();

            var testFlashcards = new List<Flashcards>
            {
                new Flashcards { /*Id = 10, FlashcardCollectionId = 15, Question = "What is 2+2?", Answer = "4"*/ },
                new Flashcards { /*Id = 11, FlashcardCollectionId = 15, Question = "What is the capital of France?", Answer = "Paris"*/ }
            };

            var testCollections = new List<FlashcardCollection<Flashcards>>
            {
                new FlashcardCollection<Flashcards>
                {
                    Flashcards = testFlashcards,
                    /*Id = 15,
                    CollectionName = "Test Collection",
                    Flashcards = testFlashcards,
                    CreatedDateTime = DateTime.Now,
                    Category = Category.ComputerScience,
                    ViewCount = 0,
                    FlashcardsAppUserId = "test-1",
                    FlashcardsAppUser = null,*/
                },
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(testCollections), Encoding.UTF8, "application/json")
            };

            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("FlashcardCollections/GetFlashcardCollections")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var mockHttpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://example.com")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient("FlashcardsAPI"))
                                 .Returns(mockHttpClient);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object)
            {
                TempData = tempData
            };

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<FlashcardCollection<Flashcards>>));
            var model = result.Model as List<FlashcardCollection<Flashcards>>;
            Assert.AreEqual(testCollections.Count, model.Count);
        }

        [TestMethod]
        public void TestCreateFlashcardCollectionView()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.CreateFlashcardCollection() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(string.IsNullOrEmpty(result.ViewName));
        }

        [TestMethod]
        public void TestCreateCollectionSuccess()
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

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object)
            {
                TempData = tempData
            };

            var collection = new FlashcardCollection<Flashcards>{};

            // Act
            var result = controller.CreateFlashcardCollection(collection) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNull(controller.TempData["ErrorMessage"], "An error occurred in the controller.");
        }

        [TestMethod]
        public void TestCreateCollectionFailure()
        {
            // Arrange - Simulating API Failure
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
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

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object)
            {
                TempData = tempData
            };

            var collection = new FlashcardCollection<Flashcards>{};

            // Act
            var result = controller.CreateFlashcardCollection(collection) as ViewResult;

            // Assert - Expecting the Error view to be returned due to API failure
            Assert.IsNotNull(result);
            Assert.AreEqual("Error", result.ViewName);

            // Arrange - Simulating Invalid Model State
            controller.ModelState.AddModelError("Error", "Model error");

            // Act
            result = controller.CreateFlashcardCollection(collection) as ViewResult;

            // Assert - Expecting the view to be returned due to invalid model state
            Assert.IsNotNull(result);
            Assert.AreEqual(collection, result.Model);
        }

        [TestMethod]
        public void TestEditCollectionSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards>
            {
                Id = 1,
            };

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
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

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.Edit(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(FlashcardCollection<Flashcards>));
            var model = result.Model as FlashcardCollection<Flashcards>;
            Assert.AreEqual(testCollection.Id, model.Id);
        }

        [TestMethod]
        public void TestEditCollectionNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

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

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.Edit(99) as RedirectToActionResult; // Using an ID that doesn't exist

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}