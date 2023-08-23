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
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public int? NewCourseRequestId { get; set; }

        public int Hour { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; } = new Subject();

        [ForeignKey(nameof(NewCourseRequestId))]
        public virtual NewCourseRequest NewCourseRequest { get; set; } = new NewCourseRequest();

    }
}