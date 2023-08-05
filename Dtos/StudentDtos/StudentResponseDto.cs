using Extensions.DateTimeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class StudentResponseDto
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FirebaseId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
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
        public string Phone { get; set; } = string.Empty;
        public string? Line { get; set; }
        public string? Email { get; set; }
        public string? School { get; set; }
        public string? CountryOfSchool { get; set; }
        public string? LevelOfStudy { get; set; }
        public string? Program { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; } = new DateTime(2030, 12, 31);
        public string? TargetUniversity { get; set; }
        public string? TargetScore { get; set; }
        public string? HogInformation { get; set; }
        public string? HealthInformation { get; set; }
        public StudentStatus status { get; set; } = StudentStatus.Inactive;

        public FilesResponseDto? ProfilePicture { get; set; }
        public ParentResponseDto? Parent { get; set; }
        public AddressResponseDto? Address { get; set; }
        public ICollection<FilesResponseDto>? AdditionalFiles { get; set; }
    }
}