using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using FlashcardsAPI.Services;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlashcardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FlashcardsStorageService _flashcardStorageService;

        public FlashcardsController(ApplicationDbContext context, FlashcardsStorageService flashcardStorage)
        {
            _context = context;
            _flashcardStorageService = flashcardStorage;
        }

        // GET: api/Flashcards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flashcards>> GetFlashcards(int id)
        {
          if (_context.Flashcards == null)
          {
              return NotFound();
          }
            var flashcards = await _context.Flashcards.FindAsync(id) ?? _flashcardStorageService.GetFlashcard(id);

            if (flashcards == null)
            {
                return NotFound();
            }

            return flashcards;
        }

        // PUT: api/Flashcards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlashcards(int id, Flashcards flashcards)
        {
            if (id != flashcards.Id)
            {
                return BadRequest();
            }

            if (_flashcardStorageService.GetFlashcard(id) != null)
            {
                _flashcardStorageService.UpdateFlashcard(id, flashcards);
                return NoContent();
            }

            _context.Entry(flashcards).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlashcardsExists(id))
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
        public async Task<ActionResult<Flashcards>> PostFlashcards(Flashcards flashcards)
        {
          if (_context.Flashcards == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Flashcards'  is null.");
          }
            
          flashcards.Id = flashcards.GetHashCode();
          _flashcardStorageService.AddFlashcard(flashcards);

          return CreatedAtAction("GetFlashcards", new { id = flashcards.Id }, flashcards);
        }

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcards(int id)
        {
            if (_context.Flashcards == null)
            {
                return NotFound();
            }

            if (_flashcardStorageService.GetFlashcard(id) != null)
            {
                _flashcardStorageService.RemoveFlashcard(id);
                return NoContent();
            }

            var flashcards = await _context.Flashcards.FindAsync(id);
            if (flashcards == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(flashcards);      
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlashcardsExists(int id)
        {
            return (_context.Flashcards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
