using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace FlashcardsApp.Models
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
