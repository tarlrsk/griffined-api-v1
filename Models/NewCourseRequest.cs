using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class NewCourseRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? CourseId { get; set; }
        public int? LevelId { get; set; }

        public int TotalHours { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual StudyCourseType StudyCourseType { get; set; }
        public virtual Method Method { get; set; }

        public virtual ICollection<NewCourseSubjectRequest> NewCourseSubjectRequests { get; set; } = new List<NewCourseSubjectRequest>();

        [ForeignKey(nameof(LevelId))]
        public virtual Level? Level { get; set; } = new Level();

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = new Course();
    }
}