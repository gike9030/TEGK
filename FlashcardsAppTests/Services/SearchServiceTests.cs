using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using System.Collections.Generic;

namespace FlashcardsApp.Tests.Services
{
    [TestClass]
    public class SearchServiceTests
    {
        [TestMethod]
        public void FilterBySearchTerm_ReturnsMatchingCollections()
        {
            // Arrange
            var searchService = new SearchService();
            var collections = new List<FlashcardCollection<Flashcards>>
            {
                new FlashcardCollection<Flashcards> { CollectionName = "Math Flashcards" },
                new FlashcardCollection<Flashcards> { CollectionName = "Science Flashcards" },
                new FlashcardCollection<Flashcards> { CollectionName = "History Flashcards" }
            };
            var searchTerm = "math";

            // Act
            var result = searchService.FilterBySearchTerm(collections, searchTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Math Flashcards", result[0].CollectionName);
        }

        [TestMethod]
        public void FilterBySearchTerm_ReturnsEmptyListForNoMatches()
        {
            // Arrange
            var searchService = new SearchService();
            var collections = new List<FlashcardCollection<Flashcards>>
            {
                new FlashcardCollection<Flashcards> { CollectionName = "Math Flashcards" },
                new FlashcardCollection<Flashcards> { CollectionName = "Science Flashcards" },
                new FlashcardCollection<Flashcards> { CollectionName = "History Flashcards" }
            };
            var searchTerm = "chemistry";

            // Act
            var result = searchService.FilterBySearchTerm(collections, searchTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
