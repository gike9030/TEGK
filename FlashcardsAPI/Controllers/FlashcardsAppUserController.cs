using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class FlashcardsAppUserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlashcardsAppUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string?>> GetFirstName(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new Response{ Status = "200", Message = user.FirstName });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string?>> GetLastName(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new Response{ Status = "200", Message = user.LastName });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<string?>> GetEmail(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new Response { Status = "200", Message = user.Email });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<FlashcardCollection<Flashcards>>?>> GetUserCollections(string id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Include(user => user.FlashcardCollections).FirstOrDefaultAsync(user => user.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.FlashcardCollections.ToList());
        }
    }
}
