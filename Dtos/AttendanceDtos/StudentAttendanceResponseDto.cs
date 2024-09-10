namespace griffined_api.Dtos.AttendanceDtos
{
    public class StudentAttendanceResponseDto
    {
        public int StudentId { get; set; }
        public string StudentFirstName { get; set; } = string.Empty;
        public string StudentLastName { get; set; } = string.Empty;
        public string StudentNickname { get; set; } = string.Empty;
        public Attendance Attendance { get; set; }
    }
}