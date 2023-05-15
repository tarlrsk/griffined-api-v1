using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class PrivateCourse
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string course { get; set; } = string.Empty;
        [Required]
        public string section { get; set; } = string.Empty;
        public string? subject { get; set; }
        public string? level { get; set; }
        [Required]
        public StudyMethod method { get; set; } = StudyMethod.onsite;
        [Required]
        public int totalHour { get; set; }
        [Required]
        public int hourPerClass { get; set; }
        private DateTime _fromDate;
        [Required]
        public string fromDate { get { return _fromDate.ToString("dd-MMMM-yyyy HH:mm:ss"); } set { _fromDate = DateTime.Parse(value); } }
        private DateTime _toDate;
        [Required]
        public string toDate { get { return _toDate.ToString("dd-MMMM-yyyy HH:mm:ss"); } set { _toDate = DateTime.Parse(value); } }
        public int reqId { get; set; }
        public bool isActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public PrivateRegistrationRequest request { get; set; } = new PrivateRegistrationRequest();
        public List<PrivateClass> privateClasses { get; set; } = new List<PrivateClass>();
        public ICollection<ExamDate>? examDates { get; set; }
    }
}