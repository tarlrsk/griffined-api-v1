using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class StudentAddingSubjectRequest
    {
        public int Id { get; set; }
        public int? StudentAddingRequestId { get; set; }
        public int? StudySubjectId { get; set; }

        [ForeignKey(nameof(StudentAddingRequestId))]
        public StudentAddingRequest StudentAddingRequest { get; set; }

        [ForeignKey(nameof(StudySubjectId))]
        public StudySubject StudySubject { get; set; }
    }
}