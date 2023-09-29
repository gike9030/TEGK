using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using FlashcardsApp.Areas.Identity.Data;
using static System.Collections.Specialized.BitVector32;

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
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
        public Category Category { get; set; }

        public string? FlashcardsAppUserId { get; set; }
        public FlashcardsAppUser? FlashcardsAppUser { get; set; }
    }

    public enum ReactionType
    {
        Hearts,
        Haha,
        Like,
        Angry
    }
    public enum Category
    {
        ComputerScience,
        Mathematics,
        Physics,
        Medicine,
        History
    }


}
