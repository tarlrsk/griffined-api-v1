using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DailyCalendarType
    {
        DELETED = 0,
        OFFICE_HOURS = 1,
        NORMAL_CLASS = 3,
        EVENT = 2,
        CANCELLED_CLASS = 4,
        MAKEUP_CLASS = 8,
        SUBSTITUTE = 16,
        HOLIDAY = 32,
    }
}