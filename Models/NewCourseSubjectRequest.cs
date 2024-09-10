using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class NewCourseSubjectRequest
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public int? NewCourseRequestId { get; set; }

        public double Hour { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        [ForeignKey(nameof(NewCourseRequestId))]
        public virtual NewCourseRequest NewCourseRequest { get; set; }

    }
}