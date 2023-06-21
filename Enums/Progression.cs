using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace griffined_api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Progression
    {
        FiftyPercent,
        HundredPercent,
        Special
    }
}