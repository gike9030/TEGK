using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class FlashcardCollection
    {
        [Key]
        public int CollectionId { get; set; }

        public string? CollectionName { get; set; }
        public int UserId { get; set; }
        public List<FlashcardViewModel>? Flashcards { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
        public int ViewCount { get; set; }
        public int Hearts {  get; set; }
        public int Haha {  get; set; }
        public int Like { get; set; }
        public int Angry { get; set; }

    }
}
