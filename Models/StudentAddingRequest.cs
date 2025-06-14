using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudentAddingRequest
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? StudyCourseId { get; set; }
        public StudyCourseType StudyCourseType { get; set; }

        [ForeignKey(nameof(RegistrationRequestId))]
        public RegistrationRequest RegistrationRequest { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public StudyCourse StudyCourse { get; set; }
        public virtual ICollection<StudentAddingSubjectRequest> StudentAddingSubjectRequests { get; set; }

    }
}