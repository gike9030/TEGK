using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
    public class Flashcards : FlashcardBase
    {
        [Required]
        public string? Question { get; set; }
        [Required]
        public string? Answer { get; set; }
        public FlashcardCollection<Flashcards>? FlashcardCollection { get; set; }
    }
}
