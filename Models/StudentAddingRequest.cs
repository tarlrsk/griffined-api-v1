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
        public int id { get; set; }
        public int? registrationRequestId { get; set; }
        public int? studyCourseId { get; set; }

        [ForeignKey(nameof(registrationRequestId))]
        public RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(studyCourseId))]
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
    }
}