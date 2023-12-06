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
    public class HomeControllerTests
    {
        [TestMethod]
        public void Search_SuccessWithResults_ReturnsSearchViewWithResults()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var testFlashcards = new List<Flashcards>
            {
                new Flashcards { },
                new Flashcards { },
            };

            var testCollections = new List<FlashcardCollection<Flashcards>>
            {
                new FlashcardCollection<Flashcards>
                {
                    Flashcards = testFlashcards,
                },
            };

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollections), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient("FlashcardsAPI"))
                                 .Returns(mockHttpClient);

            var searchResults = new List<FlashcardCollection<Flashcards>> { };

            var mockSearchService = new Mock<SearchService>();
            mockSearchService.Setup(s => s.FilterBySearchTerm(It.IsAny<List<FlashcardCollection<Flashcards>>>(), It.IsAny<string>()))
                             .Returns(searchResults);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var homeController = new HomeController(mockHttpClientFactory.Object, null, mockSearchService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = homeController.Search("query") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SearchView", result.ViewName);
            Assert.AreEqual(searchResults, result.Model);
            Assert.AreEqual("query", homeController.TempData["LastSearchQuery"]);
            Assert.AreEqual("No results found for the search query.", homeController.TempData["EmptyResults"]);
        }

        [TestMethod]
        public void Search_ApiCallFails_ReturnsErrorView()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null), Encoding.UTF8, "application/json") // Simulate API failure by returning null
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
            mockHttpClientFactory.Setup(factory => factory.CreateClient("FlashcardsAPI"))
                                         .Returns(client);

            var homeController = new HomeController(mockHttpClientFactory.Object, null, null)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            // Act
            var result = homeController.Search("query") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("ErrorView", result.ViewName);
        }

        [TestMethod]
        public void Search_FailureWithSearchQuery_ReturnsSearchViewWithAllCollections()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var testFlashcards = new List<Flashcards>
            {
                new Flashcards { },
                new Flashcards { },
            };

            var testCollections = new List<FlashcardCollection<Flashcards>>
            {
                new FlashcardCollection<Flashcards>
                {
                    Flashcards = testFlashcards,
                },
            };

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(testCollections), Encoding.UTF8, "application/json")
            };

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse);

            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://example.com")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(factory => factory.CreateClient("FlashcardsAPI"))
                                 .Returns(mockHttpClient);

            var searchResults = new List<FlashcardCollection<Flashcards>> { };

            var mockSearchService = new Mock<SearchService>();
            mockSearchService.Setup(s => s.FilterBySearchTerm(It.IsAny<List<FlashcardCollection<Flashcards>>>(), It.IsAny<string>()))
                             .Returns(searchResults);

            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            var homeController = new HomeController(mockHttpClientFactory.Object, null, mockSearchService.Object)
            {
                TempData = tempData
            };

            // Act
            var result = homeController.Search(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("SearchView", result.ViewName);
            var model = result.Model as List<FlashcardCollection<Flashcards>>;
            Assert.IsNotNull(model);
            Assert.AreEqual(testCollections.Count, model.Count);
        }

        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockSearchService = new Mock<SearchService>();

            var controller = new HomeController(mockHttpClientFactory.Object, null, mockSearchService.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }

        [TestMethod]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockSearchService = new Mock<SearchService>();

            var controller = new HomeController(mockHttpClientFactory.Object, null, mockSearchService.Object);

            // Act
            var result = controller.Privacy() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }

        [TestMethod]
        public void Error_ReturnsViewResultWithErrorViewModel()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockSearchService = new Mock<SearchService>();

            var controller = new HomeController(mockHttpClientFactory.Object, null, mockSearchService.Object)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() }
            };

            // Act
            var result = controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(ErrorViewModel));
            Assert.IsNull(result.ViewName);
        }
    }
}
