using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
	public abstract class FlashcardBase
	{
		[Key]
		public int Id { get; set; }
		public int FlashcardCollectionId { get; set; }
	}
}
