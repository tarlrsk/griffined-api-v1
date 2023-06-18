using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudySubject
    {
        public int id { get; set; }
        public ICollection<Subject> subjects { get; set; } = new List<Subject>();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
    }
}