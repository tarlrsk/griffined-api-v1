using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CancellationRequest
    {
        public int id { get; set; }
        private DateTime _requestedDate;
        public string requestedDate { get; set; } = string.Empty;
        public int classId { get; set; }
        public CancellationRole role { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
        public int teacherId { get; set; }
        public Teacher teacher { get; set; } = new Teacher();
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
        public int studyClassId { get; set; }
        public StudyClass studyClass { get; set; } = new StudyClass();
    }
}