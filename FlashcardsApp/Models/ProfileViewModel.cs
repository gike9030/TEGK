namespace FlashcardsApp.Models
{
    public class ProfileViewModel
    {
        public Profile? Profile { get; set; }
        public List<FlashcardCollection<Flashcards>>? FollowingCollections { get; set; }
    }
}
