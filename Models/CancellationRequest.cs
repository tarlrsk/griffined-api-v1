using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class CancellationRequest
    {
        public int id { get; set; }
        private DateTime _requestedDate;
        public string requestedDate { get; set; } = string.Empty;
        public int classId { get; set; }
        public _CancellationRoleEnum role { get; set; }
        public ICollection<Student> students { get; set; } = new List<Student>();
        public ICollection<Teacher> teachers { get; set; } = new List<Teacher>();
        public ICollection<StudyCourse> studyCourses { get; set; } = new List<StudyCourse>();
        public ICollection<StudyClass> studyClasses { get; set; } = new List<StudyClass>();
    }
}