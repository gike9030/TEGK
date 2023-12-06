using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsAPI.Models
{
	[PrimaryKey(nameof(FollowingUserId), nameof(FollowedUserId))]
	public class Following
	{
		[Required]
		public string? FollowingUserId { get; set; }
		public FlashcardsAppUser? FollowingUser { get; set; }
		[Required]
		public string? FollowedUserId { get; set; }
		public FlashcardsAppUser? FollowedUser { get; set; }
	}
}
