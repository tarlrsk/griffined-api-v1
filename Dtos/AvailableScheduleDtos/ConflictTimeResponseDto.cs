using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictTimeResponseDto
    {
        public int Id { get; set; }
        private TimeOnly _fromTime;
        [Required]
        public string FromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string ToTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
        public bool CurrentClass { get; set; } = false;
    }
}