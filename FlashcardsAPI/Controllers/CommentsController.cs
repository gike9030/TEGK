using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlashcardsAPI.Models;
using JWTAuthentication.NET6._0.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashcardsAPI.Services;

namespace FlashcardsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IFlashcardsAppDbService _flashcardsAppDbService;

        public CommentsController(IFlashcardsAppDbService service)
        {
            _flashcardsAppDbService = service;
        }

        // POST: api/Comments
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Comment>> PostComment([FromBody] Comment comment)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment? updated = await _flashcardsAppDbService.AddComment(comment);

            if (updated == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            Comment? comment = await _flashcardsAppDbService.GetComment(id);

            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found.");
            }

            return comment;
        }



        // PUT: api/Comments/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutComment(int id, [FromBody] Comment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest("Comment ID mismatch.");
            }

            Comment? isSuccess = await _flashcardsAppDbService.UpdateComment(id, comment);
            
            if (isSuccess == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteComment(int id)
        {
            bool isSuccess = await _flashcardsAppDbService.DeleteComment(id);

            if (isSuccess == false)
            {
                return NotFound($"Comment with ID {id} not found.");
            }

            return NoContent();
        }
    }
}
