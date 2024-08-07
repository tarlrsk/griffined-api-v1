namespace griffined_api.Models
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string? ErrorId { get; set; }
        public string? Exception { get; set; }
        public string? Source { get; set; }

        public List<string> Messages { get; set; } = new();
    }
}