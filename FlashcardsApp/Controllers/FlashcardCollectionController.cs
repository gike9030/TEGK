using FlashcardsApp.Data;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FlashcardsApp.Controllers
{
    [Authorize]
    public class FlashcardCollectionController : Controller
    {
        private readonly FlashcardsAppContext _db;

        public FlashcardCollectionController(FlashcardsAppContext db)
        {
            _db = db;
        }

        public IActionResult Index(string? sortByCategory = null)
        {
            List<FlashcardCollection<Flashcards>> flashcardCollections = _db.FlashcardCollection.Include(f => f.Flashcards).ToList();

            flashcardCollections.Sort();

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
                _db.FlashcardCollection.Add(collection);
                _db.SaveChanges();
                return RedirectToAction(actionName:"Index");
            }
            return View(collection);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var collection = _db.FlashcardCollection
                .Include(flashcardCollection => flashcardCollection.Flashcards)
                .FirstOrDefault(flashcardCollection => flashcardCollection.Id == id);
            if (collection == null)
            {
                return RedirectToAction(actionName: "Index");
            }

            return View(collection);
        }

        [HttpPost]
        public IActionResult RenameCollection(FlashcardCollection<Flashcards> cardCollection)
        {
            var collection = _db.FlashcardCollection.FirstOrDefault(flashcardCollection => flashcardCollection.Id == cardCollection.Id);

            if (collection != null)
            {
                collection.CollectionName = cardCollection.CollectionName;

                _db.Update(collection);
                _db.SaveChanges();
                return RedirectToAction(actionName: "Edit", routeValues: new { id = collection.Id });
            }
            return RedirectToAction(actionName: "Index");
        }

        [HttpPost]
        public IActionResult AddFlashcard(FlashcardCollection<Flashcards> cardCollection, string NewFlashcardFrontSide, string NewFlashcardBackSide)
        {
            var collection = _db.FlashcardCollection.FirstOrDefault(flashcardCollection => flashcardCollection.Id == cardCollection.Id);

            if (collection != null)
            {
                var newFlashcard = new Flashcards
                {
                    Question = NewFlashcardFrontSide,
                    Answer = NewFlashcardBackSide,
                    FlashcardCollectionId = collection.Id
                };

                _db.Flashcards.Add(newFlashcard);
                _db.SaveChanges();
                return RedirectToAction(actionName: "Edit", routeValues: new { id = collection.Id });
            }
            return RedirectToAction(actionName: "Index");
        }

        [HttpPost]
        public IActionResult AddFlashcardsFromFile(FlashcardCollection<Flashcards> cardCollection, IFormFile? flashcardFile)
        {
            var collection = _db.FlashcardCollection.FirstOrDefault(flashcardCollection => flashcardCollection.Id == cardCollection.Id);

            if (collection == null || flashcardFile == null || flashcardFile.Length < 1)
            {
                return RedirectToAction("Edit", collection);
            }

            using var reader = new StreamReader(flashcardFile.OpenReadStream());
            string content = reader.ReadToEnd();
            var flashcards = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in flashcards)
            {
                var parts = item.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    var newFlashcard = new Flashcards
                    {
                        Question = parts[0],
                        Answer = parts[1],
                        FlashcardCollection = collection,
                        FlashcardCollectionId = collection.Id
                    };
                    _db.Flashcards.Add(newFlashcard);
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Edit", collection);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var collection = _db.FlashcardCollection
                .Include(flashcardCollection => flashcardCollection.Flashcards)
                .FirstOrDefault(flashcardCollection => flashcardCollection.Id == id);

            if (collection == null)
            {
                return NotFound("Collection not found");
            }

            foreach (var flashcard in collection.Flashcards)
            {
                _db.Flashcards.Remove(flashcard);
            }

            _db.FlashcardCollection.Remove(collection);
            _db.SaveChanges();

            return RedirectToAction(actionName: "Index");
        }

        [HttpGet]
        public IActionResult ViewCollection(int id)
        {
            var collection = _db.FlashcardCollection
                .Include(flashcardCollection => flashcardCollection.Flashcards)
                .FirstOrDefault(flashcardCollection => flashcardCollection.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            return View(collection);
        }

        [HttpGet]
        public IActionResult EditFlashcard(int id) 
        {
            var flashcard = _db.Flashcards.FirstOrDefault(f => f.Id == id);
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
                var flashcard = _db.Flashcards.FirstOrDefault(f => f.Id == editedFlashcard.Id);
                if (flashcard == null)
                {
                    return NotFound();
                }

                flashcard.Question = editedFlashcard.Question;
                flashcard.Answer = editedFlashcard.Answer;

                _db.Entry(flashcard).State = EntityState.Modified;
                _db.SaveChanges();

                return RedirectToAction(actionName: "Edit", routeValues: new { id = flashcard.FlashcardCollectionId }); 
            }
            return View(editedFlashcard);
        }

        [HttpPost]
        public IActionResult DeleteFlashcard(int id)
        {
            var flashcard = _db.Flashcards.Find(id);
            if (flashcard == null)
            {
                return NotFound();
            }

            int collectionId = flashcard.FlashcardCollectionId;

            _db.Flashcards.Remove(flashcard);
            _db.SaveChanges();

            return RedirectToAction("Edit", new { id = collectionId });
        }

        [HttpGet]
        public IActionResult ViewCollections()
        {
            return RedirectToAction(actionName: "Index");
        }
        [HttpGet]
        public IActionResult PlayCollection(int id, int? cardIndex)
        {
            var collection = _db.FlashcardCollection
                .Include(flashcardCollection => flashcardCollection.Flashcards)
                .FirstOrDefault(flashcardCollection => flashcardCollection.Id == id);

            if (collection == null || !collection.Flashcards.Any())
            {
                TempData["Error"] = "The collection is empty or not found.";
                return RedirectToAction(actionName:"Index");
            }

            cardIndex = cardIndex ?? 0;
            if (cardIndex < 0) cardIndex = 0;
            if (cardIndex >= collection.Flashcards.Count) cardIndex = collection.Flashcards.Count - 1;

            var cardToShow = collection.Flashcards.ElementAt((int)cardIndex);
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
    }

}