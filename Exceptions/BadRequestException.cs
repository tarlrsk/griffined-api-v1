using System.Net;

namespace griffined_api.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message, null, HttpStatusCode.BadRequest)
        {
        }
    }
}