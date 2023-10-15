namespace FlashcardsAPI.Models
{
    public class Reaction<CollectionType>
    {
        public int ReactionId { get; set; }
        
        public ReactionType Type { get; set; }
        public int Count { get; set; }

        public int? FlashcardCollectionId { get; set; }
        public FlashcardCollection<CollectionType>? FlashcardCollection { get; set; }
    }
}
