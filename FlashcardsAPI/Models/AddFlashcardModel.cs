using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
    public class AddFlashcardModel
    {
        [Required]
        public string? Question { get; set; }
        [Required]
        public string? Answer { get; set; }

        public int FlashcardCollectionId { get; set; }
    }
}
