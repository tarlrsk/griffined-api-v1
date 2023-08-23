using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAddingRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? StudyCourseId { get; set; }
        public StudyCourseType StudyCourseType { get; set; }

        [ForeignKey(nameof(RegistrationRequestId))]
        public RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(StudyCourseId))]
        public StudyCourse StudyCourse { get; set; } = new StudyCourse();
        public virtual ICollection<StudentAddingSubjectRequest> StudentAddingSubjectRequests { get; set; } = new List<StudentAddingSubjectRequest>();

    }
}