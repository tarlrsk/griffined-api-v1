using System.Text.Json.Serialization;

namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DailyCalendarType
    {
        DELETED = 0,
        OFFICE_HOURS = 1,
        EVENT = 2,
        NORMAL_CLASS = 4,
        CANCELLED_CLASS = 8,
        MAKEUP_CLASS = 16,
        SUBSTITUTE = 32,
        HOLIDAY = 64,
    }
}