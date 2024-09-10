namespace griffined_api.Dtos.StudyCourseDtos
{
    public class ScheduleStudentMobileResponseDto
    {
        public int StudyCourseId { get; set; }
        public int CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string CourseSubject { get; set; } = string.Empty;
        public int StudyClassId { get; set; }
        public int ClassNo { get; set; }
        public string? Room { get; set; }
        public string Date { get; set; } = string.Empty;
        public string FromTime { get; set; } = string.Empty;
        public string ToTime { get; set; } = string.Empty;
        public ClassStatus ClassStatus { get; set; }
        public Attendance Attendance { get; set; }
        public bool IsFiftyPercent { get; set; }
        public bool IsHundredPercent { get; set; }
        public int TeacherId { get; set; }
        public string TeacherFirstName { get; set; } = string.Empty;
        public string TeacherLastName { get; set; } = string.Empty;
        public string TeacherNickname { get; set; } = string.Empty;
        public TeacherWorkType TeacherWorkType { get; set; }
    }
}