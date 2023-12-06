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
        public void TestExceptionWhenFetchCollectionsIsNull()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>();

            // Simulate a null response
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = null // Simulate a null response content
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

            //Act
            var result = controller.Index() as ViewResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("Error", result.ViewName);
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

        [TestMethod]
        public void TestRenameCollectionSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards>
            {
                Id = 1,
                CollectionName = "Original Collection",
            };

            var updatedCollection = new FlashcardCollection<Flashcards>
            {
                Id = 1,
                CollectionName = "Renamed Collection",
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            var putResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get || req.Method == HttpMethod.Put),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    return request.Method == HttpMethod.Get ? getResponse : putResponse;
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.RenameCollection(updatedCollection) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(updatedCollection.Id, result.RouteValues["id"]);
        }

        [TestMethod]
        public void TestRenameCollectionUpdateFailure()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards>
            {
                Id = 1,
                CollectionName = "Original Collection",
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            var updateFailureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    return request.Method == HttpMethod.Put ? updateFailureResponse : getResponse;
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            var collectionToUpdate = new FlashcardCollection<Flashcards>
            {
                Id = 1,
                CollectionName = "Renamed Collection",
            };

            // Act
            var result = controller.RenameCollection(collectionToUpdate) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void TestAddFlashcardSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var collection = new FlashcardCollection<Flashcards> { Id = 1 };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(collection), Encoding.UTF8, "application/json")
            };
            var postResponse = new HttpResponseMessage(HttpStatusCode.Created);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : postResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.AddFlashcard(collection, "Question", "Answer") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(collection.Id, result.RouteValues["id"]);
        }

        [TestMethod]
        public void TestAddFlashcardFailure()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var collection = new FlashcardCollection<Flashcards> { Id = 1 };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(collection), Encoding.UTF8, "application/json")
            };

            var postFailureResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : postFailureResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.AddFlashcard(collection, "Question", "Answer") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void TestAddFlashcardsFromFileSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards>
            {
                Id = 1,
                CollectionName = "Test Collection",
            };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            var postResponse = new HttpResponseMessage(HttpStatusCode.Created);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : postResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            var mockFile = new Mock<IFormFile>();
            var content = "Question1\nAnswer1\n\nQuestion2\nAnswer2";
            var fileName = "flashcards.txt";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var collection = new FlashcardCollection<Flashcards> { Id = 1 };

            // Act
            var result = controller.AddFlashcardsFromFile(collection, mockFile.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(collection.Id, result.RouteValues["id"]);
        }

        [TestMethod]
        public void TestAddFlashcardsFromFileFailure()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            var collection = new FlashcardCollection<Flashcards> { };
            IFormFile flashcardFile = null;

            // Act
            var result = controller.AddFlashcardsFromFile(collection, flashcardFile) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
        }

        [TestMethod]
        public async Task TestDeleteCollectionSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards> { Id = 1 };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            var deleteResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : deleteResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = await controller.Delete(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task TestDeleteCollectionNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = await controller.Delete(99) as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.AreEqual("Collection not found", result.Value);
        }

        [TestMethod]
        public void TestViewCollectionSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testCollection = new FlashcardCollection<Flashcards> { Id = 1, };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(getResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.ViewCollection(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(FlashcardCollection<Flashcards>));
            var model = result.Model as FlashcardCollection<Flashcards>;
            Assert.AreEqual(testCollection.Id, model.Id);
        }

        [TestMethod]
        public void TestViewCollectionNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.ViewCollection(99) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void TestEditFlashcardGetSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testFlashcard = new Flashcards { Id = 1, };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testFlashcard), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(getResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.EditFlashcard(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Flashcards));
            var model = result.Model as Flashcards;
            Assert.AreEqual(testFlashcard.Id, model.Id);
        }

        [TestMethod]
        public void TestEditFlashcardGetNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.EditFlashcard(99) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void TestEditFlashcardPostSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testFlashcard = new Flashcards { Id = 1, Question = "Original Question", Answer = "Original Answer", FlashcardCollectionId = 2 };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testFlashcard), Encoding.UTF8, "application/json")
            };
            var putResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : putResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);
            controller.ModelState.Clear();

            var editedFlashcard = new Flashcards { Id = 1, Question = "Updated Question", Answer = "Updated Answer" };

            // Act
            var result = controller.EditFlashcard(editedFlashcard) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(testFlashcard.FlashcardCollectionId, result.RouteValues["id"]);
        }

        [TestMethod]
        public void TestEditFlashcardPostNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            var editedFlashcard = new Flashcards { Id = 99, Question = "Updated Question", Answer = "Updated Answer" };

            // Act
            var result = controller.EditFlashcard(editedFlashcard) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);

            // Arrange - Simulating Invalid Model State
            controller.ModelState.AddModelError("Error", "Model error");

            // Act
            var resultModel = controller.EditFlashcard(editedFlashcard) as ViewResult;

            // Assert
            Assert.IsNotNull(resultModel);
            Assert.IsNull(resultModel.ViewName);
            Assert.IsInstanceOfType(resultModel.Model, typeof(Flashcards));
            Assert.IsFalse(resultModel.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public async Task TestDeleteFlashcardSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var testFlashcard = new Flashcards { Id = 1, FlashcardCollectionId = 2 };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testFlashcard), Encoding.UTF8, "application/json")
            };
            var deleteResponse = new HttpResponseMessage(HttpStatusCode.NoContent);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                    request.Method == HttpMethod.Get ? getResponse : deleteResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = await controller.DeleteFlashcard(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(testFlashcard.FlashcardCollectionId, result.RouteValues["id"]);
        }

        [TestMethod]
        public async Task TestDeleteFlashcardNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = await controller.DeleteFlashcard(99) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public void TestViewCollections()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            // Act
            var result = controller.ViewCollections() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void TestPlayCollectionSuccess()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var flashcards = new List<Flashcards>
            {
                new Flashcards { Id = 1, Question = "Question 1", Answer = "Answer 1" },
                new Flashcards { Id = 2, Question = "Question 2", Answer = "Answer 2" }
            };

            var testCollection = new FlashcardCollection<Flashcards> { Id = 1, Flashcards = flashcards };

            var getResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollection), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(getResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            int cardIndex = 1;

            // Act
            var result = controller.PlayCollection(1, cardIndex) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(Flashcards));
            var model = result.Model as Flashcards;
            Assert.AreEqual(flashcards[cardIndex].Id, model.Id);
            Assert.AreEqual(cardIndex, controller.ViewBag.CardIndex);
        }

        [TestMethod]
        public void TestPlayCollectionNotFound()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            var notFoundResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(notFoundResponse);

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com/")
            };

            mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            // Act
            var result = controller.PlayCollection(99, null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);

            //Arrange - Simulate a last search query being present in TempData
            string lastSearchQuery = "previous search";
            controller.TempData["LastSearchQuery"] = lastSearchQuery;

            // Act
            result = controller.PlayCollection(99, null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Search", result.ActionName);
            Assert.AreEqual(lastSearchQuery, result.RouteValues["search"]);
        }

        [TestMethod]
        public void TestUpdateElapsedTime()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);

            var elapsedTime = new ElapsedTime { };

            // Act
            var result = controller.UpdateElapsedTime(elapsedTime) as OkResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [TestMethod]
        public void TestBackWithLastSearchQuery()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);
            var tempDataProvider = Mock.Of<ITempDataProvider>();
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            string lastSearchQuery = "test query";
            controller.TempData["LastSearchQuery"] = lastSearchQuery;

            // Act
            var result = controller.Back() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Search", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual(lastSearchQuery, result.RouteValues["search"]);
        }

        [TestMethod]
        public void TestBackWithoutLastSearchQuery()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var controller = new FlashcardCollectionController(mockHttpClientFactory.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            // Act
            var result = controller.Back() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsNull(result.ControllerName);
        }
    }
}