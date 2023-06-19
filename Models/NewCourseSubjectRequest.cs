using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseSubjectRequest
    {
        public int id { get; set; }
        public int? subjectId { get; set; }
        public Subject subject { get; set; } = new Subject();
        public int newCourseRequestId { get; set; }
        public NewCourseRequest newCourseRequest { get; set; } = new NewCourseRequest();
        public int hour { get; set; }
    }
}