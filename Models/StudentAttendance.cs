using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAttendance
    {
        public int id { get; set; }
        public int? studentId { get; set; }
        public int? studyClassId { get; set; }

        public virtual Attendance attendance { get; set; }

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();

        [ForeignKey(nameof(studyClassId))]
        public virtual StudyClass studyClass { get; set; } = new StudyClass();
    }
}