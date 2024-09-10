using System.Text.Json.Serialization;


namespace griffined_api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TeacherNotificationType
    {
        MakeupClass,
        NewCourse,
        ClassCancellation,
        NewAppointment,
        NewAppointmentType,
        AppointmentCancellation,
        StudentReport
    }
}