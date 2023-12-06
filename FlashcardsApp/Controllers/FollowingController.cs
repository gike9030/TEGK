using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
	[Authorize]
	public class FollowingController : Controller
	{
		private readonly HttpClient _httpClient;

		public FollowingController(IHttpClientFactory httpClientFactory) 
		{
			_httpClient = httpClientFactory.CreateClient("FlashcardsAPI");
		}
		
		public IActionResult Follow(string followingUserId, string followedUserId)
		{

			Following following = new() {FollowingUserId = followingUserId, FollowedUserId = followedUserId};
			HttpResponseMessage resp = HttpApiService.PostToAPI(_httpClient, "/Followings/PostFollowing", following);

			if (resp.IsSuccessStatusCode)
			{
				TempData["Success"] = "User followed";
				return RedirectToAction("Index", "FlashcardCollection");
			}

            TempData["Error"] = "Failed to follow user";
            return RedirectToAction("Index", "FlashcardCollection");
        }

		public async Task<IActionResult> Unfollow(string followingUserId, string followedUserId)
		{
            var res = await HttpApiService.DeleteFromAPI(_httpClient, "/Followings/DeleteFollowing/" + followingUserId + "/" + followedUserId);

			if (res.IsSuccessStatusCode)
			{
				TempData["Success"] = "Unfollowed user successfuly";
				return RedirectToAction("Index", "Profile");
			}

			TempData["Error"] = "Failed to unfollow user";
			return RedirectToAction("Index", "Profile");
        }
	}
}
