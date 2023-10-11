namespace griffined_api.Dtos.WorkTimeDtos
{
    public class WorkTimeResponseDto
    {
        [Required]
        public Enums.DayOfWeek Day { get; set; }
        private TimeOnly _fromTime;
        [Required]
        public string FromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string ToTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
    }
}