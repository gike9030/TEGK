using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlashcardsAPI.Controllers;
using FlashcardsAPI.Services;
using FlashcardsAPI.Models;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FlashcardsAPITests.Controllers
{
    [TestClass]
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileService> _mockProfileService;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockProfileService = new Mock<IProfileService>();
            _controller = new ProfileController(_mockProfileService.Object);
        }

        [TestMethod]
        public async Task GetProfile_ExistingUserId_ReturnsOkResult()
        {
            // Arrange
            string userId = "validUserId";
            var userProfile = new Profile(); // Assuming a UserProfile class
            _mockProfileService.Setup(service => service.GetProfileAsync(userId)).ReturnsAsync(userProfile);

            // Act
            var result = await _controller.GetProfile(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetProfile_NonExistingUserId_ReturnsNotFoundResult()
        {
            // Arrange
            string userId = "nonExistingUserId";
            _mockProfileService.Setup(service => service.GetProfileAsync(userId)).ReturnsAsync((Profile)null);

            // Act
            var result = await _controller.GetProfile(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task EditDescription_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            string userId = "validUserId";
            string description = "new description";
            _mockProfileService.Setup(service => service.UpdateDescriptionAsync(userId, description)).ReturnsAsync(true);

            // Act
            var result = await _controller.EditDescription(userId, description);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task EditDescription_InvalidRequest_ReturnsBadRequestResult()
        {
            // Arrange
            string userId = "validUserId";
            string description = "new description";
            _mockProfileService.Setup(service => service.UpdateDescriptionAsync(userId, description)).ReturnsAsync(false);

            // Act
            var result = await _controller.EditDescription(userId, description);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task UploadProfilePhoto_ValidPath_ReturnsOkResult()
        {
            // Arrange
            string userId = "validUserId";
            string profilePhotoPath = "valid/path/to/photo.jpg";
            _mockProfileService.Setup(service => service.UpdateProfilePhotoPathAsync(userId, profilePhotoPath)).ReturnsAsync(true);

            // Act
            var result = await _controller.UploadProfilePhoto(userId, profilePhotoPath);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task UploadProfilePhoto_EmptyPath_ReturnsBadRequestResult()
        {
            // Arrange
            string userId = "validUserId";
            string profilePhotoPath = string.Empty;
            _mockProfileService.Setup(service => service.UpdateProfilePhotoPathAsync(userId, profilePhotoPath)).ReturnsAsync(false);

            // Act
            var result = await _controller.UploadProfilePhoto(userId, profilePhotoPath);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
