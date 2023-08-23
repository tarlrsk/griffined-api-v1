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
        public int Id { get; set; }
        public int? StudentId { get; set; }
        public int? StudyClassId { get; set; }

        public virtual Attendance Attendance { get; set; }

        [ForeignKey(nameof(StudentId))]
        public virtual Student? Student { get; set; }

        [ForeignKey(nameof(StudyClassId))]
        public virtual StudyClass? StudyClass { get; set; }
    }
}