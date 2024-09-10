using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClassStatus
    {
        NONE,
        CHECKED,
        UNCHECKED,
        PENDING_CANCELLATION,
        CANCELLED,
        DELETED,
    }
}