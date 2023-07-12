using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class AvailableTeacherResponseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string FirebaseId { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }

        [Required]
        public string Nickname { get; set; } = string.Empty;
        public string WorkType { get; set; } = string.Empty;
        public bool CurrentClass { get; set; } = false;
    }
}