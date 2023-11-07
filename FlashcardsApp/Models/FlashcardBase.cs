using System.ComponentModel.DataAnnotations;
using FlashcardsApp.Areas.Identity.Data;

namespace FlashcardsApp.Models
{
	public abstract class FlashcardBase
	{
		[Key]
		public int Id { get; set; }
		public int FlashcardCollectionId { get; set; }
	}
}
