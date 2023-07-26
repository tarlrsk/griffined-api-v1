using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudyCourseHistory
    {
        public int Id { get; set; }
        public int? StudyCourseId { get; set; }
        public int? StaffId { get; set; }

        public string Description { get; set; } = string.Empty;
        private DateTime _dateUpdated;
        public string DateUpdated { get { return _dateUpdated.ToString("dd-MMMM-yyyy HH:mm:ss"); } set { _dateUpdated = DateTime.Parse(value); } }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(StaffId))]
        public virtual Staff ByStaff { get; set; } = new Staff();
    }
}