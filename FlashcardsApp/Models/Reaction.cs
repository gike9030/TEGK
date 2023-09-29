namespace FlashcardsApp.Models
{
    public class Reaction
    {
        [Key]
        public int ReactionId { get; set; }
        
        public ReactionType Type { get; set; }
        public int Count { get; set; }

        public int? FlashcardCollectionId { get; set; }
        public FlashcardCollection? FlashcardCollection { get; set; }
    }
}
