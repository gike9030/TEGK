namespace FlashcardsAPI.Models
{
    public class EditFlashcardModel
    {
        public int Id { get; set; }
        public string? NewQuestion { get; set; }
        public string? NewAnswer { get; set; }
    }
}
