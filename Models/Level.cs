using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class Level
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }

        public string level { get; set; } = string.Empty;

        public virtual ICollection<NewCourseRequest> NewCourseRequests { get; set; }
        public virtual ICollection<StudyCourse> StudyCourses { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
    }
}