using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CancellationRequest
    {
        public int id { get; set; }
        public int classId { get; set; }
        public int? studentId { get; set; }
        public int? teacherId { get; set; }
        public int? studyCourseId { get; set; }
        public int? studyClassId { get; set; }

        private DateTime _requestedDate;
        public string requestedDate { get; set; } = string.Empty;
        public virtual CancellationRole role { get; set; }

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();

        [ForeignKey(nameof(teacherId))]
        public virtual Teacher teacher { get; set; } = new Teacher();

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(studyClassId))]
        public virtual StudyClass studyClass { get; set; } = new StudyClass();
    }
}