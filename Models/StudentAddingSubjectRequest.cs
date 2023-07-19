using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentAddingSubjectRequest
    {
        public int Id { get; set; }
        public int? StudentAddingRequestId { get; set; }
        public int? StudySubjectId { get; set; }

        [ForeignKey(nameof(StudentAddingRequestId))]
        public StudentAddingRequest StudentAddingRequest { get; set; } = new StudentAddingRequest();

        [ForeignKey(nameof(StudySubjectId))]
        public StudySubject StudySubject { get; set; } = new StudySubject();
    }
}