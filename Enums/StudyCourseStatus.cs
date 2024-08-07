using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StudyCourseStatus
    {
        Ongoing,
        Finished,
        Pending,
        NotStarted,
        Cancelled
    }
}