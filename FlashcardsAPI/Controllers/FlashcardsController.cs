using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlashcardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlashcardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Flashcards>> GetFlashcards(int id)
        {
          if (_context.Flashcards == null)
          {
              return NotFound();
          }
            var flashcards = await _context.Flashcards.FindAsync(id);

            if (flashcards == null)
            {
                return NotFound();
            }

            return flashcards;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlashcards(int id, Flashcards flashcards)
        {
            if (id != flashcards.Id)
            {
                return BadRequest();
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

        [HttpPost]
        public async Task<ActionResult<Flashcards>> PostFlashcards(Flashcards flashcards)
        {
          if (_context.Flashcards == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Flashcards'  is null.");
          }
            _context.Flashcards.Add(flashcards);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlashcards", new { id = flashcards.Id }, flashcards);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcards(int id)
        {
            if (_context.Flashcards == null)
            {
                return NotFound();
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
