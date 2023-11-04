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

        public int FlashcardCollectionId { get; set; }

        public FlashcardCollection<Flashcards>? FlashcardCollection { get; set; }
        public string? FirstName { get; set; }
        public string? UserId { get; set; }
        public FlashcardsAppUser? User { get; set; }
    }

}
