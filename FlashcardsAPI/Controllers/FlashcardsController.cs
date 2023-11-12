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
        private readonly FlashcardsStorageService _flashcardStorageService;
        private readonly IFlashcardsAppDbService _flashcardsAppDbService;

        public FlashcardsController(ApplicationDbContext context, FlashcardsStorageService flashcardStorage, IFlashcardsAppDbService service)
        {
            _flashcardsAppDbService = service;
            _context = context;
            _flashcardStorageService = flashcardStorage;
        }

        // GET: api/Flashcards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Flashcards>> GetFlashcards(int id)
        {

            Flashcards? flashcards = await _flashcardsAppDbService.GetFlashcard(id) ?? _flashcardStorageService.GetFlashcard(id);

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
            if (_flashcardStorageService.GetFlashcard(id) != null)
            {
                _flashcardStorageService.UpdateFlashcard(id, flashcards);
                return NoContent();
            }

            Flashcards? updatedFlashcard = await _flashcardsAppDbService.UpdateFlashcard(id, flashcards);

            if (updatedFlashcard == null) 
            {
                return BadRequest();
            }
            return NoContent();
        }

        // POST: api/Flashcards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Flashcards>> PostFlashcards(Flashcards flashcards)
        {       
          flashcards.Id = flashcards.GetHashCode();
          _flashcardStorageService.AddFlashcard(flashcards);

          return CreatedAtAction("GetFlashcards", new { id = flashcards.Id }, flashcards);
        }

        // DELETE: api/Flashcards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcards(int id)
        {
            if (_flashcardStorageService.GetFlashcard(id) != null)
            {
                _flashcardStorageService.RemoveFlashcard(id);
                return NoContent();
            }

            bool isSuccess = await _flashcardsAppDbService.DeleteFlashcard(id);

            if (isSuccess == false)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
