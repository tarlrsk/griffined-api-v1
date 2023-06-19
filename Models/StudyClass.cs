using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyClass
    {
        public int id { get; set; }
        public int classNumber { get; set; }
        public ClassStatus status { get; set; }
        public bool isMakeup { get; set; } = false;
        public int scheduleId { get; set; }
        public Schedule schedule { get; set; } = new Schedule();
        public int studySubjectId { get; set; }
        public StudySubject studySubject { get; set; } = new StudySubject();
        public int teacherId { get; set; }
        public Teacher teacher { get; set; } = new Teacher();
        public ICollection<StudentAttendance> attendances { get; set; } = new List<StudentAttendance>();
        public ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}