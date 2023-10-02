using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FlashcardsApp.Controllers
{
    [Authorize]
    public class FileReaderController : Controller
    {
        public IActionResult Index()
        {
            return View(new FlashcardCollection<Flashcards>());
        }

        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            if (file == null)
            {
                ModelState.AddModelError("", "Please select a file to upload.");
                return View(new FileData());
            }

            long size = file.Length;
            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var fileContent = System.IO.File.ReadAllLines(filePath);

            if (fileContent.Length < 4 || fileContent.Length % 2 == 1)
            {
                ModelState.AddModelError("", "Invalid file format. Make sure the file has the collection name, category, and pairs of question and answer.");
                return View(new FileData());
            }

            FlashcardCollection<Flashcards> flashcardCollection = new FlashcardCollection<Flashcards>
            {
                CollectionName = fileContent[0]
            };

            // Check if the category in the file is valid
            if (Enum.TryParse<Category>(fileContent[1], out var parsedCategory))
            {
                flashcardCollection.Category = parsedCategory;
            }
            else
            {
                // Handle invalid category 
                ModelState.AddModelError("", "Invalid or missing category in the uploaded file.");
                return View(new FileData());
            }

            // Loop through the rest of the lines in pairs to extract questions and answers
            for (int i = 2; i < fileContent.Length; i += 2)
            {
                Flashcards flashcard = new Flashcards
                {
                    Question = fileContent[i],
                    Answer = fileContent[i + 1]
                };
                flashcardCollection.Flashcards.Add(flashcard);
            }

            // TODO: Save the flashcardCollection to your database or wherever you need
            return RedirectToAction("YourActionName", "YourControllerName", flashcardCollection);
        }
    }
}