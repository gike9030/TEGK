using FlashcardsAPI.Models;
using FlashcardsAPI.Services;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FlashcardCollectionsController : ControllerBase
    {
        private IFlashcardsStorageService _flashcardsStorageService;
        private readonly IFlashcardCollectionService _flashcardCollectionService;

        public FlashcardCollectionsController(IFlashcardsStorageService flashcardsStorageService, IFlashcardCollectionService service)
        {
            _flashcardCollectionService = service;
            _flashcardsStorageService = flashcardsStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcardsInCollection(int id)
        {
            IEnumerable<Flashcards>? flashcards = await _flashcardCollectionService.GetFlashcardsInCollection(id);

            if (flashcards == null)
            {
                return NotFound();
            }

            List<Flashcards> tempCollection = _flashcardsStorageService.GetFlashcardsInCollection(id);
            flashcards = flashcards.Concat(tempCollection).ToList();

            foreach (Flashcards flashcard in flashcards)
            {
                flashcard.FlashcardCollection = null;
            }

            return Ok(flashcards);
        }

        // GET: api/FlashcardCollections
        [HttpGet]
        public async Task<IActionResult> GetFlashcardCollections()
        {
            var collections = await _flashcardCollectionService.GetFlashcardCollections();

            if (collections == null)
            {
                return BadRequest();
            }

            return Ok(collections);

        }

        // Not used
        [HttpPost]
        public async Task<IActionResult> GetFlashcardCollections(Category category)
        {
            IEnumerable<FlashcardCollection<Flashcards>>? collections = await _flashcardCollectionService.GetFlashcardCollectionsByCategory(category);
            
            if(collections == null)
            {
                return BadRequest();
            }

            return Ok(collections);
        }


        // GET: api/FlashcardCollections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlashcardCollection<Flashcards>>> GetFlashcardCollections(int id)
        {
            FlashcardCollection<Flashcards>? collection = await _flashcardCollectionService.GetFlashcardCollection(id);

            if (collection == null)
            {
                return NotFound();
            }


			List<Flashcards> tempCollection = _flashcardsStorageService.GetFlashcardsInCollection(id);


			collection.Flashcards = collection.Flashcards.Concat(tempCollection).ToList();

			foreach (var comment in collection.Comments)
			{
				comment.FlashcardCollection = null;
			}

			foreach (var flashcard in collection.Flashcards)
			{
				flashcard.FlashcardCollection = null;
			}

            return collection;
        }

        // PUT: api/Flashcards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutFlashcardCollection(FlashcardCollection<Flashcards> collection)
        {
            FlashcardCollection<Flashcards>? updatedCollection = await _flashcardCollectionService.UpdateFlashcardCollection(collection.Id, collection);

            if (updatedCollection == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

        // POST: api/Flashcards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FlashcardCollection<Flashcards>>> PostFlashcardCollections(FlashcardCollection<Flashcards> collection)
        {
            FlashcardCollection<Flashcards>? addedCollection = await _flashcardCollectionService.AddFlashcardsCollection(collection);

            if (addedCollection == null)
            {
                return BadRequest();
            }

            return CreatedAtAction("GetFlashcardCollections", new { id = collection.Id }, collection);
        }

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcardCollections(int id)
        {
            _flashcardsStorageService.RemoveFlashcardsFromCollection(id);

            bool? isSuccess = await _flashcardCollectionService.DeleteFlashcardCollection(id);

            if (isSuccess == false)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
