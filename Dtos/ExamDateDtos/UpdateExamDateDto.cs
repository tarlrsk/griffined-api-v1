namespace griffined_api.Dtos.ExamDateDtos
{
    public class UpdateExamDateDto
    {
        public int id { get; set; }
        private DateTime _examDate;
        public string examDate { get { return _examDate.ToString("dd/MM/yyyy HH:mm:ss"); } set { _examDate = DateTime.Parse(value); } }
        public int? studentId { get; set; }
        public int? privateCourseId { get; set; }
        public int? groupCourseId { get; set; }
    }
}