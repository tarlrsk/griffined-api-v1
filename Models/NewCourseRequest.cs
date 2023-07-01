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
        public int id { get; set; }
        public int? registrationRequestId { get; set; }
        public int? courseId { get; set; }
        public int? levelId { get; set; }

        public int totalHours { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public virtual Method method { get; set; }

        public virtual ICollection<NewCourseSubjectRequest> newCourseSubjectRequests { get; set; } = new List<NewCourseSubjectRequest>();

        [ForeignKey(nameof(levelId))]
        public virtual Level? level { get; set; } = new Level();

        [ForeignKey(nameof(registrationRequestId))]
        public virtual RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(courseId))]
        public virtual Course course { get; set; } = new Course();
    }
}