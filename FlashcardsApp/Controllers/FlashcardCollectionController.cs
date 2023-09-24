using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FlashcardsApp.Controllers
{
    public class FlashcardCollectionController : Controller
    {
        private static List<FlashcardCollection> flashcardCollections = new List<FlashcardCollection>();

        public IActionResult Index()
        {
            return View(flashcardCollections);
        }

        public IActionResult CreateFlashcardCollection()
        {
            var model = new FlashcardCollection
            {
                Flashcards = new List<FlashcardViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateFlashcardCollection(FlashcardCollection collection)
        {
            if (ModelState.IsValid)
            {
                flashcardCollections.Add(collection);
                return RedirectToAction("Index");
            }
            return View(collection);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var collection = flashcardCollections.FirstOrDefault(c => c.CollectionId == id);
            if (collection == null)
            {
                return NotFound();
            }

            if (collection.Flashcards == null)
            {
                collection.Flashcards = new List<FlashcardViewModel>();
            }

            return View(collection);
        }

        [HttpPost]
        public IActionResult Edit(FlashcardCollection collection, string NewFlashcardFrontSide, string NewFlashcardBackSide)
        {
            var existingCollection = flashcardCollections.FirstOrDefault(c => c.CollectionId == collection.CollectionId);

            // Check if the collection exists
            if (existingCollection == null)
            {
                return NotFound("Collection not found");
            }

            // Validate the new flashcard data
            if (string.IsNullOrEmpty(NewFlashcardFrontSide) || string.IsNullOrEmpty(NewFlashcardBackSide))
            {
                ModelState.AddModelError("NewFlashcardError", "Both Front Side and Back Side must be filled out.");
            }
            else
            {
                // Add the new flashcard to the existing collection
                var newFlashcard = new FlashcardViewModel
                {
                    Question = NewFlashcardFrontSide,
                    Answer = NewFlashcardBackSide
                };

                existingCollection.Flashcards ??= new List<FlashcardViewModel>();

                existingCollection.Flashcards.Add(newFlashcard);

                // Redirect to the Edit view for the updated collection
                return RedirectToAction("Edit", new { id = existingCollection.CollectionId });
            }

            // If we've reached this point, it means validation failed. 
            // Reset the Flashcards to the existing ones before returning the View.
            collection.Flashcards = existingCollection.Flashcards;
            return View(collection);
        }



    }
}
