using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FlashcardsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FlashcardsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcardCollectionById(int? id)
        {
            FlashcardCollection<Flashcards>? collection = await _db.FlashcardCollection.FirstAsync(collection => collection.Id == id);

            if (collection == null)
            {
                return NotFound();
            }
            return Ok(collection);
        }

        [HttpPost]
        public async Task<IActionResult> GetFlashcardCollections(FlashcardCollectionFilterModel filter)
        {
            List<FlashcardCollection<Flashcards>> collections = _db.FlashcardCollection.ToList().Where(collection => collection.Category == filter.Category).ToList();

            return Ok(collections);
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcardsInCollectionById(int? id)
        {
            FlashcardCollection<Flashcards>? collection = await _db.FlashcardCollection.Include(collection => collection.Flashcards).FirstAsync(flashcardCollection =>  flashcardCollection.Id == id);

            foreach (Flashcards flashcard in collection.Flashcards) 
            {
                flashcard.FlashcardCollection = null;
            }
            if (collection == null)
            {
                return NotFound();
            }
            
            return Ok(collection.Flashcards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcardCollection(AddFlashcardCollectionModel collection)
        {
            try
            {
                _db.FlashcardCollection.Add(new FlashcardCollection<Flashcards> { CollectionName = collection.CollectionName, CreatedDateTime = collection.CreatedDateTime, Category = collection.Category, FlashcardsAppUserId = collection.FlashcardsAppUserId });
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFlashcardCollection(int? id)
        {
            FlashcardCollection<Flashcards>? collection = await _db.FlashcardCollection.Include(collection => collection.Flashcards).FirstAsync(_ => _.Id == id);

            try
            {
                foreach(Flashcards flashcard in collection.Flashcards)
                {
                    _db.Remove(flashcard);
                }
                _db.Remove(collection);
                _db.SaveChanges();
            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> RenameFlashcardCollectionById(int id, string name)
        {
            FlashcardCollection<Flashcards> collection = await _db.FlashcardCollection.FirstAsync(collection => collection.Id == id);

            try
            {
                collection.CollectionName = name;
                _db.FlashcardCollection.Update(collection);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcardById(int id)
        {
            Flashcards flashcard = await _db.Flashcards.FirstAsync(fc => fc.Id == id);

            if (flashcard == null)
            {
                return BadRequest();
            }

            return Ok(flashcard);
        }
        
        [HttpPut]
        public async Task<IActionResult> EditFlashcard(EditFlashcardModel newFlashcard)
        {
            Flashcards flashcard = await _db.Flashcards.FirstAsync(_ => _.Id == newFlashcard.Id);

            try
            {
                flashcard.Question = newFlashcard.NewQuestion;
                flashcard.Answer = newFlashcard.NewAnswer;
                _db.Flashcards.Update(flashcard);
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcard(AddFlashcardModel? flashcard)
        {
            try
            {
                _db.Flashcards.Add(new Flashcards { Question = flashcard.Question, Answer = flashcard.Answer, FlashcardCollectionId = flashcard.FlashcardCollectionId });
                _db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFlashcardById(int? id)
        {
            Flashcards? flashcard = await _db.Flashcards.FirstAsync(flashcard => flashcard.Id == id);

            if (flashcard == null)
            {
                return BadRequest();
            }

            _db.Remove(flashcard);
            _db.SaveChanges();
            return Ok();
        }
    }
}
