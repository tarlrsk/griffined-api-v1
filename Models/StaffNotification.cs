using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StaffNotification
    {
        public int id { get; set; }
        public int? staffId { get; set; }
        public int? studyCourseId { get; set; }

        public DateTime dateCreated { get; set; } = DateTime.Now;

        public bool hasRead { get; set; }

        public virtual StaffNotificationType type { get; set; }

        [ForeignKey(nameof(staffId))]
        public virtual Staff staff { get; set; } = new Staff();

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();
    }
}