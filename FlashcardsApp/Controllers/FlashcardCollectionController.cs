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
        public IActionResult Index(string sortByCategory = null)
        {
            IQueryable<FlashcardCollection<Flashcards>> flashcardCollections = _db.FlashcardCollection.Include(f => f.Flashcards);

            if (!string.IsNullOrEmpty(sortByCategory))
            {
                if (Enum.TryParse(sortByCategory, out Category categoryValue))
                {
                    flashcardCollections = flashcardCollections.Where(f => f.Category == categoryValue);
                }
            }
            ViewBag.CurrentSort = sortByCategory;

            return View(flashcardCollections.ToList());
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            return View(collection);
        }

        [HttpPost]
        public IActionResult Edit(FlashcardCollection<Flashcards> cardCollection, string NewFlashcardFrontSide, string NewFlashcardBackSide, IFormFile flashcardFile)
        {
            var collection = _db.FlashcardCollection
            .Include(flashcardCollection => flashcardCollection.Flashcards)
            .FirstOrDefault(flashcardCollection => flashcardCollection.Id == cardCollection.Id);

            if (collection != null)
            {
                collection.CollectionName = cardCollection.CollectionName;

                
                if (!string.IsNullOrEmpty(NewFlashcardFrontSide) && !string.IsNullOrEmpty(NewFlashcardBackSide))
                {
                    var newFlashcard = new Flashcards
                    {
                        Question = NewFlashcardFrontSide,
                        Answer = NewFlashcardBackSide,
                        FlashcardCollection = collection,
                        FlashcardCollectionId = collection.Id
                    };
                    _db.Flashcards.Add(newFlashcard);
                }
                
                if (flashcardFile != null && flashcardFile.Length > 0)
                {
                    using (var reader = new StreamReader(flashcardFile.OpenReadStream()))
                    {
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
                    }
                }

                _db.Update(collection);
                _db.SaveChanges();
            }

            return View(collection);
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

            // Remove all associated flashcards first
            foreach (var flashcard in collection.Flashcards)
            {
                _db.Flashcards.Remove(flashcard);
            }

            _db.FlashcardCollection.Remove(collection);
            _db.SaveChanges();

            return RedirectToAction("Index");
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
        public IActionResult EditFlashcard(int id) // id is the Flashcards Model's Id
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

                return RedirectToAction("Edit", new { id = flashcard.FlashcardCollectionId }); // Redirect back to the collection edit page
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
            return RedirectToAction("Index");
        }

    }


}
