using FlashcardsApp.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        public int FlashcardCollectionId { get; set; }

        public FlashcardCollection<Flashcards>? FlashcardCollection { get; set; }
        public string? FirstName { get; set; }
        public string? UserId { get; set; }
        public FlashcardsAppUser? User { get; set; }
    }
}

