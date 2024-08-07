namespace griffined_api.Dtos.AppointentDtos
{
    public class AppointmentHistoryResponseDto
    {
        public AppointmentHistoryType RecordType { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Record { get; set; } = string.Empty;
    }
}