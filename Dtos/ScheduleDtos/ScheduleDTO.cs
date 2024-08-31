using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class CreateScheduleDTO
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }
    }

    #region Appointment

    public class AppointmentScheduleDTO : GeneratedAppointmentScheduleDTO
    {
        [JsonProperty("scheduleId")]
        public int ScheduleId { get; set; }
    }

    public class CheckAvailableAppointmentScheduleDTO
    {
        [JsonProperty("teacherIds")]
        public IEnumerable<int> TeacherIds { get; set; }

        [JsonProperty("dates")]
        public IEnumerable<string> Dates { get; set; }

        [JsonProperty("days")]
        public IEnumerable<string>? Days { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("appointmentType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AppointmentType AppointmentType { get; set; }

        [JsonProperty("currentSchedules")]
        public IEnumerable<GeneratedAppointmentScheduleDTO>? CurrentSchedules { get; set; }
    }

    public class GeneratedAppointmentScheduleDTO
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        [JsonConverter(typeof(StringEnumConverter))]
        public string Day { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("hour")]
        public decimal Hour { get; set; }

        [JsonProperty("accumulatedHour")]
        public decimal AccumulatedHour { get; set; }

        [JsonProperty("scheduleType")]
        public ScheduleType ScheduleType { get; set; }

        [JsonProperty("appointmentType")]
        public AppointmentType? AppointmentType { get; set; }

        [JsonProperty("scheduleStatus")]
        public AppointmentSlotStatus ScheduleStatus { get; set; }
    }

    public class AvailableAppointmentDTO
    {
        [JsonProperty("generatedSchedules")]
        public IEnumerable<GeneratedAppointmentScheduleDTO> GeneratedSchedules { get; set; }

        [JsonProperty("conflictedSchedules")]
        public IEnumerable<ConflictScheduleDTO> ConflictedSchedules { get; set; }
    }

    public class CheckAvailableTeacherAppointmentDTO
    {
        [JsonProperty("appointmentId")]
        public int AppointmentId { get; set; }

        [JsonProperty("teacherIds")]
        public IEnumerable<int> TeacherIds { get; set; }

        [JsonProperty("currentSchedules")]
        public IEnumerable<GeneratedAppointmentScheduleDTO> CurrentSchedules { get; set; }
    }

    public class AvailableDTO
    {
        [JsonProperty("isAvailabled")]
        public bool IsAvailabled { get; set; }
    }

    #endregion

    public class ConflictScheduleDTO
    {
        [JsonProperty("conflictedStudents")]
        public IEnumerable<StudentNameResponseDto> ConflictedStudents { get; set; }

        [JsonProperty("conflictedTeachers")]
        public IEnumerable<TeacherNameResponseDto> ConflictedTeachers { get; set; }

        [JsonProperty("conflictedScheduleIds")]
        public IEnumerable<int> ConflictedScheduleIds { get; set; }

        [JsonProperty("dates")]
        public IEnumerable<string> Dates { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("appointmentId")]
        public int? AppointmentId { get; set; }

        [JsonProperty("studyCourseId")]
        public int? StudyCourseId { get; set; }

        [JsonProperty("studySubjects")]
        public IEnumerable<ConflictedStudySubjectDTO> StudySubjects { get; set; }

        [JsonProperty("courseName")]
        public string? CourseName { get; set; }
    }

    public class ConflictedStudySubjectDTO
    {
        [JsonProperty("studySubjectId")]
        public int StudySubjectId { get; set; }

        [JsonProperty("studySubjectName")]
        public string StudySubjectName { get; set; }
    }

    #region Class

    public class CheckAvailableClassScheduleDTO
    {
        [JsonProperty("studentIds")]
        public IEnumerable<int> StudentIds { get; set; }

        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("subjectId")]
        public int SubjectId { get; set; }

        [JsonProperty("levelId")]
        public int? LevelId { get; set; }

        [JsonProperty("teacherId")]
        public int TeacherId { get; set; }

        [JsonProperty("dates")]
        public IEnumerable<string> Dates { get; set; }

        [JsonProperty("days")]
        public IEnumerable<string>? Days { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("currentSchedules")]
        public IEnumerable<GeneratedAvailableClassScheduleDTO>? CurrentSchedules { get; set; }
    }

    public class AvailableClassScheduleDTO
    {
        [JsonProperty("generatedSchedules")]
        public IEnumerable<GeneratedAvailableClassScheduleDTO> GeneratedSchedules { get; set; }

        [JsonProperty("conflictedSchedules")]
        public IEnumerable<ConflictScheduleDTO> ConflictedSchedules { get; set; }
    }

    public class GeneratedAvailableClassScheduleDTO
    {
        [JsonProperty("teacher")]
        public TeacherNameResponseDto Teacher { get; set; }

        [JsonProperty("additionalHours")]
        public AdditionalHours? AdditionalHours { get; set; }

        [JsonProperty("courseId")]
        public int CourseId { get; set; }

        [JsonProperty("courseName")]
        public string CourseName { get; set; }

        [JsonProperty("subjectId")]
        public int SubjectId { get; set; }

        [JsonProperty("subjectName")]
        public string SubjectName { get; set; }

        [JsonProperty("levelId")]
        public int? LevelId { get; set; }

        [JsonProperty("levelName")]
        public string? LevelName { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        [JsonConverter(typeof(StringEnumConverter))]
        public string Day { get; set; }

        [JsonProperty("fromTime")]
        public TimeSpan FromTime { get; set; }

        [JsonProperty("toTime")]
        public TimeSpan ToTime { get; set; }

        [JsonProperty("hour")]
        public decimal Hour { get; set; }

        [JsonProperty("accumulatedHour")]
        public decimal AccumulatedHour { get; set; }

        [JsonProperty("scheduleType")]
        public ScheduleType ScheduleType { get; set; }

        [JsonProperty("scheduleStatus")]
        public ClassStatus ScheduleStatus { get; set; }
    }

    public class AdditionalHours
    {
        [JsonProperty("type")]
        public TeacherWorkType TeacherWorkType { get; set; }

        [JsonProperty("hours")]
        public double Hours { get; set; }
    }

    #endregion
}
