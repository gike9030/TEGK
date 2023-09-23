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
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(NewFlashcardFrontSide) && !string.IsNullOrEmpty(NewFlashcardBackSide))
                {
                    var newFlashcard = new FlashcardViewModel
                    {
                        Question = NewFlashcardFrontSide,
                        Answer = NewFlashcardBackSide
                    };

                    if (collection.Flashcards == null)
                    {
                        collection.Flashcards = new List<FlashcardViewModel>();
                    }

                    collection.Flashcards.Add(newFlashcard);
                }

                var existingCollection = flashcardCollections.FirstOrDefault(c => c.CollectionId == collection.CollectionId);
                if (existingCollection != null)
                {
                    existingCollection.CollectionName = collection.CollectionName;
                    existingCollection.FlashcardsAppUserId = collection.FlashcardsAppUserId;

                    if (existingCollection.Flashcards == null)
                    {
                        existingCollection.Flashcards = new List<FlashcardViewModel>();
                    }

                    foreach (var flashcard in collection.Flashcards)
                    {
                        existingCollection.Flashcards.Add(flashcard);
                    }
                }
                else
                {
                    return NotFound("Collection not found");
                }

                return RedirectToAction("Edit", new { id = collection.CollectionId });
            }

            return View(collection);
        }
    }
}
