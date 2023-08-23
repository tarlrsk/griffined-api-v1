using System.Net;

namespace griffined_api.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message, null, HttpStatusCode.NotFound)
        {
        }
    }
}