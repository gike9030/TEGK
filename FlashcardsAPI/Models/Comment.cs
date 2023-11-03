using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlashcardsAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        // Foreign key to associate the comment with a flashcard collection
        public int FlashcardCollectionId { get; set; }

        // Navigation property to FlashcardCollection
        public FlashcardCollection<Flashcards>? FlashcardCollection { get; set; }
        // If you have a user associated with the comment
        public string? UserId { get; set; }
        public FlashcardsAppUser? User { get; set; }
    }

}
