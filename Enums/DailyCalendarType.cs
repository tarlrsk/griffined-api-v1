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
        OFFICE_HOURS = 0,
        NORMAL_CLASS = 2,
        EVENT = 1,
        CANCELLED_CLASS = 4,
        MAKEUP_CLASS = 8,
        SUBSTITUTE = 16,
        HOLIDAY = 32,
    }
}