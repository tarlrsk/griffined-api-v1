using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictTimeResponseDto
    {
        public int id { get; set; }
        private TimeOnly _fromTime;
        [Required]
        public string fromTime { get { return _fromTime.ToString("HH:mm"); } set { _fromTime = TimeOnly.Parse(value); } }
        private TimeOnly _toTime;
        [Required]
        public string toTime { get { return _toTime.ToString("HH:mm"); } set { _toTime = TimeOnly.Parse(value); } }
        public bool currentClass { get; set; } = false;
    }
}