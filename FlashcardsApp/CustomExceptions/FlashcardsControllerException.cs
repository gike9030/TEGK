namespace FlashcardsApp.CustomExceptions
{
    public class FlashcardsControllerException : Exception
    {
        public FlashcardsControllerException() { }

        public FlashcardsControllerException(string message) : base(message) { }

        public FlashcardsControllerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
