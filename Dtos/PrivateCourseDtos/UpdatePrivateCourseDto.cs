using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateCourseDtos
{
    public class UpdatePrivateCourseDto
    {
        [Required]
        public int id { get; set; }
        public int reqId { get; set; }
        [Required]
        public string course { get; set; } = string.Empty;
        [Required]
        public string section { get; set; } = string.Empty;
        [Required]
        public string subject { get; set; } = string.Empty;
        [Required]
        public string level { get; set; } = string.Empty;
        [Required]
        public StudyMethod method { get; set; }
        [Required]
        public int totalHour { get; set; }
        [Required]
        public int hourPerClass { get; set; }
        private DateTime _fromDate;
        [Required]
        public string fromDate { get { return _fromDate.ToString("dd-MMMM-yyyy"); } set { _fromDate = DateTime.Parse(value); } }
        private DateTime _toDate;
        [Required]
        public string toDate { get { return _toDate.ToString("dd-MMMM-yyyy"); } set { _toDate = DateTime.Parse(value); } }
        public List<UpdatePrivateClassDto> privateClasses { get; set; } = new List<UpdatePrivateClassDto>();
    }
}