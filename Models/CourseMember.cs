using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CourseMember
    {
        public int id { get; set; }
        public ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
        public ICollection<Student> students { get; set; } = new List<Student>();
    }
}