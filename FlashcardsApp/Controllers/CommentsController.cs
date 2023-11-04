using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using FlashcardsApp.Areas.Identity.Data;

namespace FlashcardsApp.Controllers
{
    public class CommentsController : Controller
    {
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;
        private readonly UserManager<FlashcardsAppUser> _userManager;

        public CommentsController(UserManager<FlashcardsAppUser> userManager)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = _baseAddress
            };
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/comments"); 

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var comments = JsonConvert.DeserializeObject<List<Comment>>(json);

                    return View(comments);
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return Unauthorized();
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            catch (HttpRequestException)
            {
                return View("Error");
            }
            catch (JsonException)
            {
                return View("Error");
            }
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int FlashcardCollectionId, string Content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("Error");
            }
            Comment comment = new Comment
            {
                FlashcardCollectionId = FlashcardCollectionId,
                Content = Content,
                UserId = _userManager.GetUserId(User),
                FirstName = user.FirstName
            };

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(comment);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/comments", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "FlashcardCollection");
                }
                else
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid input. Please check the data.");
                    }
                    else
                    {
                        return View("Error", comment); 
                    }
                }
            }

            return View(comment);
        }


        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"comments/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(jsonResponse);
                return View(comment);
            }

            return NotFound();
        }

       
        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"comments/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(jsonResponse);
                return View(comment);
            }

            return NotFound();
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,FlashcardCollectionId,UserId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var commentJson = JsonConvert.SerializeObject(comment);
                var content = new StringContent(commentJson, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"comments/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Failed to update the comment. Please try again later.");
                }
            }

            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"comments/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var comment = JsonConvert.DeserializeObject<Comment>(jsonResponse);
                return View(comment);
            }

            return NotFound();
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"comments/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Failed to delete the comment. Please try again later.");
                return View("Delete"); 
            }
        }
    }
}
