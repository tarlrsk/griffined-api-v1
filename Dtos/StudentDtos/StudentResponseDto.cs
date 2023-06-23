using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudentDtos
{
    public class GetStudentDto
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string StudentId { get { return dateCreated.ToString("yy", System.Globalization.CultureInfo.GetCultureInfo("en-GB")) + (id % 10000).ToString("0000"); } }
        [Required]
        public string firebaseId { get; set; } = string.Empty;
        [Required]
        public string title { get; set; } = string.Empty;
        [Required]
        public string fName { get; set; } = string.Empty;
        [Required]
        public string lName { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        [Required]
        public string nickname { get; set; } = string.Empty;
        public string? profilePicture { get; set; }
        private DateTime _dob;
        [Required]
        public string dob
        {
            get { return _dob.ToString("dd-MMMM-yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { _dob = DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture); }
        }
        public int age
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
        public string phone { get; set; } = string.Empty;
        public string? line { get; set; }
        public string? email { get; set; }
        public string? school { get; set; }
        public string? countryOfSchool { get; set; } 
        public string? levelOfStudy { get; set; }
        public string? program { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public string? targetUni { get; set; }
        public string? targetScore { get; set; }
        public string? hogInfo { get; set; }
        public string? healthInfo { get; set; }
        public bool isActive { get; set; } = true;
        public ParentDtoResponse? parent { get; set; }
        public AddressRequestDto? address { get; set; }
        public ICollection<StudentAdditionalFilesResponseDto>? additionalFiles { get; set; }
    }
}