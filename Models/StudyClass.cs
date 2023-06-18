using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyClass
    {
        public int id { get; set; }
        public int classNumber { get; set; }
        public _ClassStatusEnum status { get; set; }
        public bool isMakeup { get; set; } = false;
        public int scheduleId { get; set; }
        public Schedule schedule { get; set; } = new Schedule();
        public ICollection<StudySubject> studySubjects { get; set; } = new List<StudySubject>();
        public ICollection<Teacher> teachers { get; set; } = new List<Teacher>();
    }
}