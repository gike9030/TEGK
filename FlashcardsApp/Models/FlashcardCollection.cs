using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using FlashcardsApp.Areas.Identity.Data;

namespace FlashcardsApp.Models
{
    public class FlashcardCollection
    {
        [Key]
        public int CollectionId { get; set; }

        [Required]
        public string CollectionName { get; set; }
        public ICollection<FlashcardViewModel>? Flashcards { get; set; } = new List<FlashcardViewModel>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public int Hearts {  get; set; }
        public int Haha {  get; set; }
        public int Like { get; set; }
        public int Angry { get; set; }

        public string FlashcardsAppUserId { get; set; }
        public FlashcardsAppUser FlashcardsAppUser { get; set; }
    }
}
