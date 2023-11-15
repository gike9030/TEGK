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
    }
}