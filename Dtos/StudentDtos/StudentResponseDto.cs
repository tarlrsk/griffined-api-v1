using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class StudentResponseDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FirebaseId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        private DateTime _dob;
        public string DOB
        {
            get { return _dob.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { _dob = DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture); }
        }
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
        public DateTime D1ateCreated { get; set; } = DateTime.Now;
        public string? TargetUniversity { get; set; }
        public string? TargetScore { get; set; }
        public string? HogInformation { get; set; }
        public string? HealthInformation { get; set; }
        public bool IsActive { get; set; } = true;

        public ProfilePictureResponseDto? ProfilePicture { get; set; }
        public ParentResponseDto? Parent { get; set; }
        public AddressResponseDto? Address { get; set; }
        public ICollection<StudentAdditionalFilesResponseDto>? AdditionalFiles { get; set; }
    }
}