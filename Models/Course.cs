using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string course { get; set; } = string.Empty;

        public virtual ICollection<StudyCourse> StudyCourses { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Level> Levels { get; set; }
        public virtual ICollection<NewCourseRequest> NewCourseRequests { get; set; }
    }
}