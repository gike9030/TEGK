using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.CustomExceptions;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace FlashcardsApp.Controllers
{
    public class ReactionsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly UserManager<FlashcardsAppUser> _userManager;

        public ReactionsController(IHttpClientFactory httpClientFactory, UserManager<FlashcardsAppUser> userManager)
        {
            _httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> ToggleReaction(int collectionId, ReactionType reactionType)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not logged in.");
            }

            var requestUri = $"api/Reactions/ToggleReaction?collectionId={collectionId}&reactionType={reactionType}&userId={userId}";
            var response = await _httpClient.PostAsync(requestUri, null);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log the error
                // Redirect to an error page or display an error message on the same page
                TempData["Error"] = $"Failed to toggle reaction. Status code: {response.StatusCode}, Content: {errorContent}";
                return RedirectToAction("Error", new { collectionId = collectionId }); // Replace with actual action/controller
            }

            // Redirect back to the page that displays the collection and reactions
            return RedirectToAction("Index", "FlashcardCollection", new { collectionId = collectionId }); // Replace with actual action/controller
        }


        [HttpGet]
        public async Task<IActionResult> GetReactionCounts(int collectionId)
        {
            // Build the API endpoint URL
            var apiUrl = $"api/Reactions/GetReactionCounts?collectionId={collectionId}";

            // Send a GET request to the API
            var response = await _httpClient.GetAsync(apiUrl);

            // Ensure the HTTP response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response as a string and return it directly
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            else
            {
                // If the call was not successful, log the error and handle accordingly
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new FlashcardsControllerException($"Error fetching reaction counts. Status code: {response.StatusCode}, Content: {errorContent}", response.StatusCode);
            }
        }


    }



}
