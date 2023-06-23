using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Comment
    {
        public int id { get; set; }
        public int? registrationRequestId { get; set; }
        public int? staffId { get; set; }

        public string comment { get; set; } = String.Empty;
        public DateTime createdDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(registrationRequestId))]
        public virtual RegistrationRequest registrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(staffId))]
        public virtual Staff staff { get; set; } = new Staff();
    }
}