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
        public IActionResult Index()
        {
            List<FlashcardCollection> flashcardCollections = _db.FlashcardCollection.Include(flashcardCollection => flashcardCollection.Flashcards).ToList();

            return View(flashcardCollections);
        }

        public IActionResult CreateFlashcardCollection()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFlashcardCollection(FlashcardCollection collection)
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
        public IActionResult Edit(FlashcardCollection cardCollection, string NewFlashcardFrontSide, string NewFlashcardBackSide)
        {
            var collection = _db.FlashcardCollection
            .Include(flashcardCollection => flashcardCollection.Flashcards)
            .FirstOrDefault(flashcardCollection => flashcardCollection.Id == cardCollection.Id);

            if (ModelState.IsValid && collection != null)
            {
                collection.CollectionName = cardCollection.CollectionName;

                var newFlashcard = new FlashcardViewModel
                {
                    Question = NewFlashcardFrontSide,
                    Answer = NewFlashcardBackSide,
                    FlashcardCollection = collection,
                    FlashcardCollectionId = collection.Id
                };

                _db.FlashcardViewModel.Add(newFlashcard);
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
                _db.FlashcardViewModel.Remove(flashcard);
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
        public IActionResult EditFlashcard(int id) // id is the FlashcardViewModel's Id
        {
            var flashcard = _db.FlashcardViewModel.FirstOrDefault(f => f.Id == id);
            if (flashcard == null)
            {
                return NotFound();
            }
            return View(flashcard);
        }
        [HttpPost]
        public IActionResult EditFlashcard(FlashcardViewModel editedFlashcard)
        {
            if (ModelState.IsValid)
            {
                var flashcard = _db.FlashcardViewModel.FirstOrDefault(f => f.Id == editedFlashcard.Id);
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
            var flashcard = _db.FlashcardViewModel.Find(id);
            if (flashcard == null)
            {
                return NotFound();
            }

            int collectionId = flashcard.FlashcardCollectionId;

            _db.FlashcardViewModel.Remove(flashcard);
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
