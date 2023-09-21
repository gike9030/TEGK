using System.ComponentModel.DataAnnotations;

namespace FlashcardsApp.Models
{
    public class FlashcardViewModel
    {
        [Key]
        public int FlashcardId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

    }
}
