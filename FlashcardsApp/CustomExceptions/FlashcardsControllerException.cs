using System;
using System.Net;

namespace FlashcardsApp.CustomExceptions
{
    public class FlashcardsControllerException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public FlashcardsControllerException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public FlashcardsControllerException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public FlashcardsControllerException(string message, Exception innerException, HttpStatusCode statusCode)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public override string ToString()
        {
            return $"HTTP Status Code: {StatusCode}\n{base.ToString()}";
        }
    }
}
