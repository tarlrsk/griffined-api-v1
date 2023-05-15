using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace griffined_api
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Attendance
    {
        None,
        Present,
        Absent,
        Late
    }
}