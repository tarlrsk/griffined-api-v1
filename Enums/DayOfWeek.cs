using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomDayOfWeek
    {
        //
        // Summary:
        //     Indicates Sunday.
        Sunday = 0,
        //
        // Summary:
        //     Indicates Monday.
        Monday = 1,
        //
        // Summary:
        //     Indicates Tuesday.
        Tuesday = 2,
        //
        // Summary:
        //     Indicates Wednesday.
        Wednesday = 3,
        //
        // Summary:
        //     Indicates Thursday.
        Thursday = 4,
        //
        // Summary:
        //     Indicates Friday.
        Friday = 5,
        //
        // Summary:
        //     Indicates Saturday.
        Saturday = 6
    }
}