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
        public int? StudyCourseId { get; set; }
        public int? StudySubjectId { get; set; }
        public int? TeacherId { get; set; }

        public int ClassNumber { get; set; }

        public virtual ClassStatus Status { get; set; }
        public bool IsMakeup { get; set; } = false;
        public bool IsFiftyPercent { get; set; }
        public bool IsHundredPercent { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public virtual Schedule Schedule { get; set; } = new Schedule();

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(StudySubjectId))]
        public virtual StudySubject StudySubject { get; set; } = new StudySubject();

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = new Teacher();

        public virtual ICollection<StudentAttendance> Attendances { get; set; } = new List<StudentAttendance>();
        public virtual ICollection<ClassCancellationRequest> ClassCancellationRequests { get; set; } = new List<ClassCancellationRequest>();
        public virtual ICollection<StudyCourseHistory> StudyCourseHistories { get; set; } = new List<StudyCourseHistory>();
        public virtual ICollection<TeacherShift> TeacherShifts { get; set; } = new List<TeacherShift>();
    }
}