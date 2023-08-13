using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ClassStatus
    {
        None,
        Check,
        Unchecked,
        PendingCancellation,
        Cancelled,
        Deleted,
    }
}