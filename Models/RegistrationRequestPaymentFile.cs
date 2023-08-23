using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class RegistrationRequestPaymentFile
    {
        public int Id { get; set; }
        public int? RegistrationRequestId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty; // specific path to the file

        [ForeignKey(nameof(RegistrationRequestId))]
        public virtual RegistrationRequest? RegistrationRequest { get; set; }
    }
}