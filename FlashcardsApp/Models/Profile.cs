namespace FlashcardsApp.Models
{
    public class Profile
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? Description { get; set; }
        public List<FlashcardCollection<Flashcards>>? FlashcardCollections { get; set; }
    }
}
