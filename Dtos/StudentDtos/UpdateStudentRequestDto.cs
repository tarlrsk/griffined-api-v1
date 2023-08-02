using Extensions.DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class UpdateStudentRequestDto
    {
        [Required]
        public string StudentCode { get; set; } = string.Empty;
        [Required]
        public string FirebaseId { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        [Required]
        public string Nickname { get; set; } = string.Empty;

        public string DOB { get; set; } = string.Empty;
        public int Age
        {
            get
            {
                int _age = 0;
                _age = DateTime.Now.Subtract(DOB.ToDateTime()).Days;
                _age /= 365;
                return _age;
            }
        }
        [Required]
        public string Phone { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? School { get; set; }
        public string? CountryOfSchool { get; set; }
        public string? LevelOfStudy { get; set; }
        public string? Program { get; set; }
        public string? TargetUniversity { get; set; }
        public string? TargetScore { get; set; }
        public string? HogInformation { get; set; }
        public string? HealthInformation { get; set; }

        public ParentRequestDto? Parent { get; set; }
        public AddressRequestDto? Address { get; set; }
    }
}