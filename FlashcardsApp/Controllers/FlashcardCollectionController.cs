using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using FlashcardsApp.Data;
using FlashcardsApp.Models;
using FlashcardsApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FlashcardsApp.Controllers
{
    [Authorize]
    public class FlashcardCollectionController : Controller
    {
        private readonly Uri _baseAddress = new("https://localhost:7296/api");
        private readonly HttpClient _httpClient;

        public FlashcardCollectionController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = _baseAddress
            };
        }

        public IActionResult Index(string? sortByCategory = null, string? search = null)
        {

            List<FlashcardCollection<Flashcards>>? flashcardCollections = HttpApiService.GetFromAPI<List<FlashcardCollection<Flashcards>>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections");

            flashcardCollections.Sort();

            TempData.Clear();


            if (!string.IsNullOrEmpty(sortByCategory))
            {
                if (Enum.TryParse(sortByCategory, out Category categoryValue))
                {
                    flashcardCollections = flashcardCollections.Where(f => f.Category == categoryValue).ToList();
                }
            }
            ViewBag.CurrentSort = sortByCategory;

            return View(flashcardCollections);
        }

        public IActionResult CreateFlashcardCollection()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFlashcardCollection(FlashcardCollection<Flashcards> collection)
        {

            if (ModelState.IsValid)
            {
                HttpResponseMessage resp = HttpApiService.PostToAPI(_httpClient, "/FlashcardCollections/PostFlashcardCollections", collection);
                
                if (resp.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                return View(collection);
            }
            return View(collection);
            

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", id);
            
            if (collection == null)
            {
                return RedirectToAction("Index");
            }

            return View(collection);
        }

        [HttpPost]
        public IActionResult RenameCollection(FlashcardCollection<Flashcards> cardCollection)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/" + cardCollection.Id);

            collection.CollectionName = cardCollection.CollectionName;

            HttpResponseMessage response = HttpApiService.PutToAPI(_httpClient, "/FlashcardCollections/PutFlashcardCollection", collection);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = cardCollection.Id });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddFlashcard(FlashcardCollection<Flashcards> cardCollection, string NewFlashcardFrontSide, string NewFlashcardBackSide)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", cardCollection.Id);

            if (collection != null)
            {
                var newFlashcard = new Flashcards
                {
                    Question = NewFlashcardFrontSide,
                    Answer = NewFlashcardBackSide,
                    FlashcardCollectionId = collection.Id
                };

                HttpResponseMessage response = HttpApiService.PostToAPI(_httpClient, "/Flashcards", newFlashcard);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Edit", new { id = collection.Id });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddFlashcardsFromFile(FlashcardCollection<Flashcards> cardCollection, IFormFile? flashcardFile)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", cardCollection.Id);

            if (collection == null || flashcardFile == null || flashcardFile.Length < 1)
            {
                return RedirectToAction("Edit", collection);
            }

            FlashcardFileReader fileReader = new();
            List<Flashcards> flashcardsList = fileReader.ReadFromFile(flashcardFile);

            foreach (Flashcards flashcard in flashcardsList)
            {
                flashcard.FlashcardCollectionId = collection.Id;
                HttpApiService.PostToAPI(_httpClient, "/Flashcards", flashcard);
            }

            return RedirectToAction("Edit", collection);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", id);

            if (collection == null)
            {
                return NotFound("Collection not found");
            }

            await HttpApiService.DeleteFromAPI(_httpClient, "/FlashcardCollections/DeleteFlashcardCollections/", id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ViewCollection(int id)
        {
            var collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        [HttpGet]
        public IActionResult EditFlashcard(int id) 
        {
            Flashcards? flashcard = HttpApiService.GetFromAPI<Flashcards>(_httpClient, "/Flashcards/", id);
            if (flashcard == null)
            {
                return NotFound();
            }
            return View(flashcard);
        }

        [HttpPost]
        public IActionResult EditFlashcard(Flashcards editedFlashcard)
        {
            if (ModelState.IsValid)
            {
                Flashcards? flashcard = HttpApiService.GetFromAPI<Flashcards>(_httpClient, "/Flashcards/", editedFlashcard.Id);

                if (flashcard == null)
                {
                    return NotFound();
                }

                flashcard.Question = editedFlashcard.Question;
                flashcard.Answer = editedFlashcard.Answer;

                HttpApiService.PutToAPI(_httpClient, "/Flashcards/" + flashcard.Id, flashcard);


                return RedirectToAction("Edit", new { id = flashcard.FlashcardCollectionId }); 
            }
            return View(editedFlashcard);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var flashcard = HttpApiService.GetFromAPI<Flashcards>(_httpClient, "/Flashcards/", id);

            if (flashcard == null)
            {
                return NotFound();
            }

            await HttpApiService.DeleteFromAPI(_httpClient, "/Flashcards/", id);

            return RedirectToAction("Edit", new { id = flashcard.FlashcardCollectionId });
        }

        [HttpGet]
        public IActionResult ViewCollections()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult PlayCollection(int id, int? cardIndex)
        {
            FlashcardCollection<Flashcards>? collection = HttpApiService.GetFromAPI<FlashcardCollection<Flashcards>>(_httpClient, "/FlashcardCollections/GetFlashcardCollections/", id);

            if (collection == null || !collection.Flashcards.Any())
            {
                TempData["ErrorMessage"] = "The selected collection is empty!";
                if (TempData["LastSearchQuery"] != null)
                {
                    string lastSearchQuery = TempData["LastSearchQuery"].ToString();
                    return RedirectToAction("Search", new { search = lastSearchQuery });
                }

                return RedirectToAction("Index");
            }

            cardIndex ??= 0;
            if (cardIndex < 0) cardIndex = 0;
            if (cardIndex >= collection.Flashcards.Count) cardIndex = collection.Flashcards.Count - 1;

            Flashcards cardToShow = collection.Flashcards.ElementAt((int)cardIndex);
            cardToShow.FlashcardCollection = collection;
            ViewBag.CardIndex = cardIndex;
            ViewBag.IsFirstCard = cardIndex == 0;
            ViewBag.IsLastCard = cardIndex == collection.Flashcards.Count - 1;

            return View(cardToShow);
        }

        [HttpPost]
        public IActionResult UpdateElapsedTime([FromBody] ElapsedTime elapsedTime)
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult Search(string? search)
        {
            var allCollections = HttpApiService.GetFromAPI<List<FlashcardCollection<Flashcards>>?>(_httpClient, "/FlashcardCollections/GetFlashcardCollections");

            if (TempData["ErrorMessage"] is string errorMessage)
            {
                ViewBag.ErrorMessage = errorMessage;
            }

            if (allCollections == null || !allCollections.Any())
            {
                return View("ErrorView");
            }

            TempData["LastSearchQuery"] = search;

            if (!string.IsNullOrEmpty(search))
            {
                var pattern = Regex.Escape(search);
                var regex = new Regex(pattern, RegexOptions.IgnoreCase);

                var matchingCollections = allCollections.Where(collection => regex.IsMatch(collection.CollectionName)).ToList();

                if (!matchingCollections.Any())
                {
                    ViewBag.SearchErrorMessage = "No results found for the search query.";
                }

                return View("SearchView", matchingCollections);
            }

            return View("SearchView", allCollections);
        }

        [HttpGet]
        public IActionResult Back()
        {
            if (TempData["LastSearchQuery"] != null)
            {
                string lastSearchQuery = TempData["LastSearchQuery"].ToString();
                return RedirectToAction("Search", new { search = lastSearchQuery });
            }

            return RedirectToAction("Index");
        }

    }
}