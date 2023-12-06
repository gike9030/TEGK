using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashcardsApp.CustomExceptions;
using System;
using System.Net;

namespace FlashcardsApp.Tests.CustomExceptions
{
    [TestClass]
    public class FlashcardsControllerExceptionTests
    {
        [TestMethod]
        public void Constructor_WithStatusCode_ShouldSetStatusCodeProperty()
        {
            // Arrange
            var statusCode = HttpStatusCode.BadRequest;

            // Act
            var exception = new FlashcardsControllerException(statusCode);

            // Assert
            Assert.AreEqual(statusCode, exception.StatusCode);
        }

        [TestMethod]
        public void Constructor_WithMessageAndStatusCode_ShouldSetMessageAndStatusCodeProperties()
        {
            // Arrange
            var message = "Test exception message";
            var statusCode = HttpStatusCode.NotFound;

            // Act
            var exception = new FlashcardsControllerException(message, statusCode);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(statusCode, exception.StatusCode);
        }

        [TestMethod]
        public void Constructor_WithMessageInnerExceptionAndStatusCode_ShouldSetMessageInnerExceptionAndStatusCodeProperties()
        {
            // Arrange
            var message = "Test exception message";
            var innerException = new Exception("Inner exception message");
            var statusCode = HttpStatusCode.InternalServerError;

            // Act
            var exception = new FlashcardsControllerException(message, innerException, statusCode);

            // Assert
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
            Assert.AreEqual(statusCode, exception.StatusCode);
        }

        [TestMethod]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var message = "Test exception message";
            var statusCode = HttpStatusCode.BadRequest;
            var innerException = new Exception("Inner exception message");

            var exception = new FlashcardsControllerException(message, innerException, statusCode);

            // Act
            var result = exception.ToString();

            // Assert
            var expectedString = $"{exception}";
            Assert.AreEqual(expectedString, result);
        }
    }
}
