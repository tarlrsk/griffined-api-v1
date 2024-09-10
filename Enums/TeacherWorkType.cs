using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TeacherWorkType
    {
        NORMAL,
        OVERTIME,
        SPECIAL
    }
}