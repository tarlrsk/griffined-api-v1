using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PreferredDayDtos
{
    public class PreferredDayResponseDto
    {
        public string Day { get; set; } = string.Empty;
        private TimeOnly _fromTime;
        public string FromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        public string ToTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
    }
}