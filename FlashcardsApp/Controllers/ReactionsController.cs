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
                
                TempData["Error"] = $"Failed to toggle reaction. Status code: {response.StatusCode}, Content: {errorContent}";
                return RedirectToAction("Error", new { collectionId = collectionId }); 
            }

            
            return RedirectToAction("Index", "FlashcardCollection", new { collectionId = collectionId }); 
        }
    }
}
