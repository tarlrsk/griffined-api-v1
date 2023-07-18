using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudySubject
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public int? StudyCourseId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; } = new Subject();

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        public virtual ICollection<StudySubjectMember> StudySubjectMember { get; set; } = new List<StudySubjectMember>();
        public virtual ICollection<StudyClass> StudyClasses { get; set; } = new List<StudyClass>();
    }
}