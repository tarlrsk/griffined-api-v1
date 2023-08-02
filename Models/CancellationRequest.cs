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
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? StudyCourseId { get; set; }
        public int? StudyClassId { get; set; }

        public DateTime RequestedDate { get; set; }
        public virtual CancellationRole Role { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = new Student();

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher Teacher { get; set; } = new Teacher();

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(StudyClassId))]
        public virtual StudyClass StudyClass { get; set; } = new StudyClass();
    }
}