namespace griffined_api.Models
{
    public class ServiceResponse<T>
    {
        public int StatusCode { get; set; }

        public T? Data { get; set; }
        public bool Success { get; set; } = true;

        public ResponseStatus Message { get; set; } = ResponseStatus.Success;
    }
}