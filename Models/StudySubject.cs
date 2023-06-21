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
        public int id { get; set; }
        public int? subjectId { get; set; }
        public int? studyCourseId { get; set; }

        [ForeignKey(nameof(subjectId))]
        public virtual Subject subject { get; set; } = new Subject();

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();

        public virtual ICollection<StudyClass> studyClasses { get; set; } = new List<StudyClass>();
    }
}