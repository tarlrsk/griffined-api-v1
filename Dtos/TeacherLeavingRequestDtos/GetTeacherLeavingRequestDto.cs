namespace griffined_api.Dtos.TeacherLeavingRequestDtos
{
    public class GetTeacherLeavingRequestDto
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int teacherId { get; set; }
        private DateTime _fromDate;
        [Required]
        public string fromDate { get { return _fromDate.ToString("dd/MM/yyyy"); } set { _fromDate = DateTime.Parse(value); } }
        private DateTime _toDate;
        [Required]
        public string toDate { get { return _toDate.ToString("dd/MM/yyyy"); } set { _toDate = DateTime.Parse(value); } }
        private TimeOnly _fromTime;
        [Required]
        public string fromTime { get { return _fromTime.ToString(); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string toTime { get { return _toTime.ToString(); } set { _toTime = TimeOnly.Parse(value); } }
        public string? teacherRemark { get; set; }
        public string? EARemark { get; set; }
        public string? OARemark { get; set; }
    }
}