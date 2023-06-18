using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAttendance
    {
        public int id { get; set; }
        public _AttendanceEnum attendance { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
        public ICollection<StudyClass> studyClasses { get; set; } = new List<StudyClass>();
    }
}