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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FollowingsController : ControllerBase
    {
        private readonly IFollowingService _followingService;

        public FollowingsController(IFollowingService followingService)
        {
            _followingService = followingService;
        }

		[HttpGet("{id}")]
		public async Task<ActionResult<Following>> GetUsersFollowedByUserWithId(string id)
		{
            List<Following>? following = await _followingService.GetUsersFollowedByUserWithId(id);

            if (following == null)
            {
                return NotFound();
            }

			return Ok(following);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Following>> GetUsersFollowingUserWithId(string id)
		{
			List<Following>? following = await _followingService.GetUsersThatFollowUserWithId(id);

			if (following == null)
			{
				return NotFound();
			}

			return Ok(following);
		}

		// POST: api/Followings
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
        public async Task<ActionResult<Following>> PostFollowing(Following following)
        {
            bool success = await _followingService.AddFollowing(following);

            if (success)
            {
                return Ok();
            }

            return BadRequest(); 
        }

        // DELETE: api/Followings/5
        [HttpDelete("{followingId}/{followedId}")]
        public async Task<IActionResult> DeleteFollowing(string? followingId, string? followedId)
        {
            bool deleted = await _followingService.DeleteFollowing(followingId, followedId);
            if (deleted)
            {
                return NoContent();
            }

            return BadRequest();
        }
    }
}
