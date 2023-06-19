using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAttendance
    {
        public int id { get; set; }
        public Attendance attendance { get; set; }
        public int studentId { get; set; }
        public Student student { get; set; } = new Student();
        public int attendanceId { get; set; }
        public StudentAttendance studentAttendance { get; set; } = new StudentAttendance();
    }
}