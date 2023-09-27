using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using FlashcardsApp.Areas.Identity.Data;

namespace FlashcardsApp.Models
{
    public class FlashcardCollection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CollectionName { get; set; }
        public ICollection<Flashcards> Flashcards { get; set; } = new List<Flashcards>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public int Hearts { get; set; }
        public int Haha { get; set; }
        public int Like { get; set; }
        public int Angry { get; set; }

        [Required]
        public Category Category { get; set; }

        public string? FlashcardsAppUserId { get; set; }
        public FlashcardsAppUser? FlashcardsAppUser { get; set; }
    }
    public enum Category
    {
        Matematika,
        Fizika,
        Medicina,
        Istorija,
        Politika
    }
}
