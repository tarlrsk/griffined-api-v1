namespace griffined_api.Dtos.ScheduleDtos
{
    public class TeacherScheduleGroup
    {
        public Teacher? Teacher { get; set; }
        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}