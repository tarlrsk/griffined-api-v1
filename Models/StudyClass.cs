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
        public int Id { get; set; }
        public int? ScheduleId { get; set; }
        public int? StudySubjectId { get; set; }
        public int? TeacherId { get; set; }

        public int ClassNumber { get; set; }

        public string? Room { get; set; }
        public virtual ClassStatus Status { get; set; }
        public bool IsMakeup { get; set; } = false;

        [ForeignKey(nameof(ScheduleId))]
        public virtual Schedule Schedule { get; set; } = new Schedule();

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; } = new StudySubject();

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = new Teacher();

        public virtual ICollection<StudentAttendance> Attendances { get; set; } = new List<StudentAttendance>();
        public virtual ICollection<CancellationRequest> CancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}