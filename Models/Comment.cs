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
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }
        public int? StaffId { get; set; }

        public string comment { get; set; } = String.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest RegistrationRequest { get; set; } = new RegistrationRequest();

        [ForeignKey(nameof(StaffId))]
        public virtual Staff Staff { get; set; } = new Staff();
    }
}