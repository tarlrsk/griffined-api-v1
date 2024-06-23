using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace griffined_api.Dtos.TeacherDtos
{
    public class AddTeacherDto
    {
        [Required]
        [JsonProperty("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [JsonProperty("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("fullName")]
        public string FullName { get { return FirstName + " " + LastName; } }

        [Required]
        [JsonProperty("nickname")]
        public string Nickname { get; set; } = string.Empty;

        [Required]
        [JsonProperty("phone")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonProperty("line")]
        public string Line { get; set; } = string.Empty;

        [JsonProperty("mandays")]
        public List<MandayRequestDto> Mandays { get; set; } = new List<MandayRequestDto>();

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;

        [JsonProperty("isPartTime")]
        public bool IsPartTime { get; set; } = false;

    }
}