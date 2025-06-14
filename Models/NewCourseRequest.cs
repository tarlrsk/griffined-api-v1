using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class NewCourseRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? CourseId { get; set; }
        public int? LevelId { get; set; }

        public double TotalHours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual StudyCourseType StudyCourseType { get; set; }
        public virtual Method Method { get; set; }
        public int? StudyCourseId { get; set; }

        public virtual ICollection<NewCourseSubjectRequest> NewCourseSubjectRequests { get; set; }

        [ForeignKey(nameof(LevelId))]
        public virtual Level? Level { get; set; }

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }
        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse? StudyCourse { get; set; }
    }
}