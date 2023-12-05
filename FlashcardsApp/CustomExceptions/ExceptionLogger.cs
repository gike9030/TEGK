using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FlashcardsApp.CustomExceptions
{
    public static class ExceptionLogger
    {
        private static readonly string LogFilePath = "exceptions.txt";
        private static string _lastLoggedExceptionHash = string.Empty;

        public static void LogException(Exception ex)
        {
            string currentExceptionHash = GetHashString(ex.Message + ex.StackTrace);
            if (_lastLoggedExceptionHash == currentExceptionHash)
            {
                return;
            }

            _lastLoggedExceptionHash = currentExceptionHash;

            using (StreamWriter writer = new StreamWriter(LogFilePath, true))
            {
                writer.WriteLine($"[DateTime: {DateTime.Now}]");
                writer.WriteLine($"[Exception Type: {ex.GetType().FullName}]");
                writer.WriteLine($"[Message: {ex.Message}]");
                if (ex is FlashcardsControllerException flashEx)
                {
                    writer.WriteLine($"[Status Code: {flashEx.StatusCode}]");
                }
                writer.WriteLine($"[StackTrace: {ex.StackTrace}]");
                writer.WriteLine(new string('-', 50));
                writer.WriteLine();
            }
        }

        private static string GetHashString(string inputString)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputString));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
