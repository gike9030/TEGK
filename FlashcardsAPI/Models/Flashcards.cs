﻿using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
    public class Flashcards
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Question { get; set; }
        [Required]
        public string? Answer { get; set; }

        public int FlashcardCollectionId { get; set; }
        public FlashcardCollection<Flashcards>? FlashcardCollection { get; set; }
    }
    public struct ElapsedTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }

}