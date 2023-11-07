namespace FlashcardsAPI.Models
{
    public class Reaction<T> where T : FlashcardBase
    {
        public int ReactionId { get; set; }
        
        public ReactionType Type { get; set; }
        public int Count { get; set; }

        public int? FlashcardCollectionId { get; set; }
		public FlashcardCollection<T>? FlashcardCollection { get; set; }
    }
}