using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyClass
    {
        public int id { get; set; }
        public int? scheduleId { get; set; }
        public int? studySubjectId { get; set; }
        public int? teacherId { get; set; }

        public int classNumber { get; set; }

        public virtual ClassStatus status { get; set; }
        public bool isMakeup { get; set; } = false;

        [ForeignKey(nameof(scheduleId))]
        public virtual Schedule schedule { get; set; } = new Schedule();

        [ForeignKey(nameof(studySubjectId))]
        public virtual StudySubject studySubject { get; set; } = new StudySubject();

        [ForeignKey(nameof(teacherId))]
        public virtual Teacher teacher { get; set; } = new Teacher();

        public virtual ICollection<StudentAttendance> attendances { get; set; } = new List<StudentAttendance>();
        public virtual ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}