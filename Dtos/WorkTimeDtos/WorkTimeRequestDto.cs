namespace griffined_api.Dtos.WorkTimeDtos
{
    public class WorkTimeRequestDto
    {

        [Required]
        public string day { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        [Required]
        public string fromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string toTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
    }
}