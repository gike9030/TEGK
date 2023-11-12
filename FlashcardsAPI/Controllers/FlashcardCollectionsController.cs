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
        private FlashcardsStorageService _flashcardsStorageService;
        private readonly ApplicationDbContext _context;
        private readonly IFlashcardsAppDbService _flashcardsCollectionService;

        public FlashcardCollectionsController(ApplicationDbContext context, FlashcardsStorageService flashcardsStorageService, IFlashcardsAppDbService service)
        {
            _flashcardsCollectionService = service;
            _flashcardsStorageService = flashcardsStorageService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcardsInCollection(int? id)
        {
            FlashcardCollection<Flashcards>? collection = await _context.FlashcardCollection.Include(collection => collection.Flashcards).FirstOrDefaultAsync(flashcardCollection => flashcardCollection.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            List<Flashcards> tempCollection = _flashcardsStorageService.GetFlashcardsInCollection(id);
            collection.Flashcards = collection.Flashcards.Concat(tempCollection).ToList();

            foreach (Flashcards flashcard in collection.Flashcards)
            {
                flashcard.FlashcardCollection = null;
            }

            return Ok(collection.Flashcards);
        }

        // GET: api/FlashcardCollections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlashcardCollection<Flashcards>>>> GetFlashcardCollections()
        {
            if (_context.FlashcardCollection == null)
            {
                return NotFound();
            }

            var collectionsWithComments = await _context.FlashcardCollection
                .Include(collection => collection.Comments) 
                .ToListAsync();

            return collectionsWithComments;
        }

        [HttpPost]
        public IActionResult GetFlashcardCollections(Category category)
        {
            List<FlashcardCollection<Flashcards>> collections = _context.FlashcardCollection.ToList().Where(collection => collection.Category == category).ToList();

            return Ok(collections);
        }


        // GET: api/FlashcardCollections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlashcardCollection<Flashcards>>> GetFlashcardCollections(int id)
        {
            FlashcardCollection<Flashcards>? collection = await _flashcardsCollectionService.GetFlashcardCollection(id);

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
            FlashcardCollection<Flashcards>? updatedCollection = await _flashcardsCollectionService.UpdateFlashcardCollection(collection.Id, collection);

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
            FlashcardCollection<Flashcards>? addedCollection = await _flashcardsCollectionService.AddFlashcardsCollection(collection);

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

            bool? isSuccess = await _flashcardsCollectionService.DeleteFlashcardCollection(id);

            if (isSuccess == false)
            {
                return BadRequest();
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
