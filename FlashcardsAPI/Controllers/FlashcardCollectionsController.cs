using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class FlashcardCollectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlashcardCollectionsController(ApplicationDbContext context)
        {
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

            foreach (Flashcards flashcard in collection.Flashcards)
            {
                flashcard.FlashcardCollection = null;
            }

            return Ok(collection.Flashcards);
        }

        // GET: api/Flashcards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlashcardCollection<Flashcards>>>> GetFlashcardCollections()
        {
            if (_context.FlashcardCollection == null)
            {
                return NotFound();
            }
            return await _context.FlashcardCollection.ToListAsync();
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
            if (_context.FlashcardCollection == null)
            {
                return NotFound();
            }

            var collection = await _context.FlashcardCollection.Include(c => c.Flashcards).FirstOrDefaultAsync(c => c.Id == id);



            if (collection == null)
            {
                return NotFound();
            }

            foreach(var flashcard in collection.Flashcards) 
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
            _context.Entry(collection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlashcardCollectionExists(collection.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Flashcards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FlashcardCollection<Flashcards>>> PostFlashcardCollections(FlashcardCollection<Flashcards> collection)
        {
            if (_context.FlashcardCollection == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Flashcards'  is null.");
            }
            _context.FlashcardCollection.Add(collection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlashcardCollections", new { id = collection.Id }, collection);
        }

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcardCollections(int id)
        {
            if (_context.FlashcardCollection == null)
            {
                return NotFound();
            }
            var collection = await _context.FlashcardCollection.Include(c => c.Flashcards).FirstOrDefaultAsync(c => c.Id == id);

            if (collection == null)
            {
                return NotFound();
            }

            foreach (var flashcard in collection.Flashcards) 
            {
                _context.Flashcards.Remove(flashcard);
            }

            _context.FlashcardCollection.Remove(collection);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlashcardCollectionExists(int id)
        {
            return (_context.FlashcardCollection?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
