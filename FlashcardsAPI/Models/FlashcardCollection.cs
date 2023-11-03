using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
    public class FlashcardCollection<FlashcardType>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? CollectionName { get; set; }
        public ICollection<FlashcardType> Flashcards { get; set; } = new List<FlashcardType>();
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public ICollection<Reaction<FlashcardType>> Reactions { get; set; } = new List<Reaction<FlashcardType>>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [Required(ErrorMessage = "Please select a category.")]
        public Category? Category { get; set; }
        public string? FlashcardsAppUserId { get; set; }
        public FlashcardsAppUser? FlashcardsAppUser { get; set; }
    }
}
