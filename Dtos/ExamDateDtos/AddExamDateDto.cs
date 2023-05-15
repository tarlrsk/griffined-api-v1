namespace griffined_api.Dtos.ExamDateDtos
{
    public class AddExamDateDto
    {
        private DateTime _examDate;
        public string examDate { get { return _examDate.ToString("dd/MM/yyyy HH:mm:ss"); } set { _examDate = DateTime.Parse(value); } }
    }
}