namespace griffined_api.Dtos.AttendanceDtos
{
    public class UpdateAttendanceRequestDto
    {
        [Required]
        public int StudentId { get; set; }
        [Required]
        public Attendance Attendance { get; set; }
    }
}