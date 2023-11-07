namespace FlashcardsApp.CustomExceptions
{
    using System;
    using System.IO;

    public static class ExceptionLogger
    {
        private static readonly string LogFilePath = "exceptions.txt";

        public static void LogException(Exception ex)
        {
            // Create or append to the log file
            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"[DateTime: {DateTime.Now}]");
                writer.WriteLine($"[Exception Type: {ex.GetType().FullName}]");
                writer.WriteLine($"[Message: {ex.Message}]");
                writer.WriteLine($"[StackTrace: {ex.StackTrace}]");
                writer.WriteLine(new string('-', 50));
                writer.WriteLine();
            }
        }
    }

}

