using System.ComponentModel.DataAnnotations;

namespace FlashcardsAPI.Models
{
    public class AddFlashcardCollectionModel
    {
        [Required]
        public string? CollectionName { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Please select a category.")]
        public Category? Category { get; set; }
        public string? FlashcardsAppUserId { get; set; }
    }
}
