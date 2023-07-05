using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class UpdateStudentRequestDto
    {
        [Required]
        public int StudentId { get; set; }
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
        public string? ProfilePicture { get; set; }
        private DateTime _dob;
        [Required]
        public string DOB { get { return _dob.ToString("dd-MMMM-yyyy HH:mm:ss"); } set { _dob = DateTime.Parse(value); } }
        public int Age
        {
            get
            {
                int _age = 0;
                _age = DateTime.Now.Subtract(_dob).Days;
                _age /= 365;
                return _age;
            }
        }
        [Required]
        public string Phone { get; set; } = string.Empty;
        public string? Line { get; set; }
        public string? Email { get; set; }
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
        public ICollection<UpdateStudentAdditionalFilesRequestDto>? AdditionalFiles { get; set; }
    }
}