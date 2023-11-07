using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using static System.Collections.Specialized.BitVector32;

namespace FlashcardsAPI.Models
{
    public class FlashcardCollection<FlashcardType> : IComparable<FlashcardCollection<FlashcardType>> where FlashcardType : FlashcardBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CollectionName { get; set; }
        public ICollection<FlashcardType> Flashcards { get; set; } = new List<FlashcardType>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public ICollection<Reaction<FlashcardType>> Reactions { get; set; } = new List<Reaction<FlashcardType>>();

        [Required(ErrorMessage = "Please select a category.")]
        public Category? Category { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public string? FlashcardsAppUserId { get; set; }
        public FlashcardsAppUser? FlashcardsAppUser { get; set; }

        public int CompareTo(FlashcardCollection<FlashcardType>? other)
        {
            if (other == null)
            {
                return 1;
            }
            return other.CreatedDateTime.CompareTo(this.CreatedDateTime);
        }
    }
}
