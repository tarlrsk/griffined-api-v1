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
        public int Id { get; set; }
        public int? StaffId { get; set; }
        public int? StudyCourseId { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public bool hasRead { get; set; }

        public virtual StaffNotificationType Type { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual Staff Staff { get; set; } = new Staff();

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();
    }
}