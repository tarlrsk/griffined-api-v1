using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppointmentType
    {
        MEETING,
        TRAINING,
        DEMO,
        PREPARE,
        OBSERVE,
        HOLIDAY,
    }
}