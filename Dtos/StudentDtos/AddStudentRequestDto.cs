using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class AddStudentRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Nickname { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        private DateTime _dob;
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
        public ICollection<AddStudentAdditionalFilesRequestDto>? AdditionalFiles { get; set; }
    }
}