using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateRegReqInformationDtos
{
    public class AddPrivateRegReqInformationDto
    {
        public string course { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string level { get; set; } = string.Empty;
        public int totalHour { get; set; }
        public StudyMethod method { get; set; }
        public int hourPerClass { get; set; }
        private DateTime _fromDate;
        public string fromDate { get { return _fromDate.ToString("dd-MMMM-yyyy"); } set { _fromDate = DateTime.Parse(value); } }
        private DateTime _toDate;
        public string toDate { get { return _toDate.ToString("dd-MMMM-yyyy"); } set { _toDate = DateTime.Parse(value); } }
        public List<AddPreferredDayDto> preferredDays { get; set; } = new List<AddPreferredDayDto>();
    }
}