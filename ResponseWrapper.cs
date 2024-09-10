using System.Net;
using Newtonsoft.Json;

namespace griffined_api
{
    public static class ResponseWrapper
    {
        public static Wrapper<T> Success<T>(HttpStatusCode httpStatusCode, T data = default) where T : class
        {
            return new Wrapper<T>
            {
                Code = $"{(int)httpStatusCode}",
                Message = "Success",
                Data = data
            };
        }

        public static Wrapper<string> Success(HttpStatusCode httpStatusCode)
        {
            return new Wrapper<string>
            {
                Code = $"{(int)httpStatusCode}",
                Message = "Success",
                Data = null
            };
        }
    }

    public class Wrapper<T> where T : class
    {
        /// <summary>
        /// Response code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Response message in TH
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Response data
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }

        /// <summary>
        /// Exception stack trace used when env is not production 
        /// </summary>
        [JsonProperty("stackTrace", NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }
    }
}
