namespace griffined_api.Dtos.ScheduleDtos
{
    public class UpdateRoomRequestDto
    {
        [Required]
        public int ScheduleId { get; set; }
        public string Room { get; set; } = string.Empty;
    }
}