namespace FlashcardsApp.Models
{
    public class FileData
    {
        private string _id;
        public string? FileName { get { return _id; } set {
                _id = value + "kazkas";
                    } }
        public int FileSize { get; set; }
        public string? Text { get; set; }
    }
}
