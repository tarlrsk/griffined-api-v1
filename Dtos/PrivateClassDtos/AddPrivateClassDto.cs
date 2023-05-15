using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateClassDtos
{
    public class AddPrivateClassDto
    {
        public string room { get; set; } = string.Empty;
        [Required]
        public StudyMethod method { get; set; } = StudyMethod.onsite;
        private DateTime _date;
        [Required]
        public string date { get { return _date.ToString("dd-MMMM-yyyy"); } set { _date = DateTime.Parse(value); } }
        private TimeOnly _fromTime;
        [Required]
        public string fromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string toTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
        public List<AddStudentPrivateClassDto> studentPrivateClasses { get; set; } = new List<AddStudentPrivateClassDto>();
        public AddTeacherPrivateClassDto teacherPrivateClass { get; set; } = new AddTeacherPrivateClassDto();
    }
}