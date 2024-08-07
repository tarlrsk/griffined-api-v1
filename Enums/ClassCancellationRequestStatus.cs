using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClassCancellationRequestStatus
    {
        None,
        Completed,
        Rejected,
    }
}