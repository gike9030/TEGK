using FlashcardsApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace FlashcardsApp.Controllers
{
    public class FileController : Controller
    {
        public List<Flashcards> ReadFlashcardsFromFile(IFormFile flashcardFile)
        {
            var flashcardList = new List<Flashcards>();

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
                        Answer = parts[1]
                    };
                    flashcardList.Add(newFlashcard);
                }
            }

            return flashcardList;
        }
    }
}