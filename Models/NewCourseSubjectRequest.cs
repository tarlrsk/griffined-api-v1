using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseSubjectRequest
    {
        public int id { get; set; }
        public int? subjectId { get; set; }
        public int? newCourseRequestId { get; set; }

        public int hour { get; set; }

        [ForeignKey(nameof(subjectId))]
        public virtual Subject subject { get; set; } = new Subject();

        [ForeignKey(nameof(newCourseRequestId))]
        public virtual NewCourseRequest newCourseRequest { get; set; } = new NewCourseRequest();

    }
}