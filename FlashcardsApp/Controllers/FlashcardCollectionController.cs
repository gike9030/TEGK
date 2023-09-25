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
    }
}
