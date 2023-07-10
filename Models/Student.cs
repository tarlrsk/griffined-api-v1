using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        public string StudentCode { get; set; } = string.Empty;
        public string FirebaseId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Nickname { get; set; } = string.Empty;
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

        public StudentStatus Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public virtual ProfilePicture? ProfilePicture { get; set; }
        public virtual Parent? Parent { get; set; }
        public virtual Address? Address { get; set; }
        public virtual StudentAttendance? Attendance { get; set; }

        public virtual ICollection<StudentAdditionalFile>? AdditionalFiles { get; set; }
        public virtual ICollection<RegistrationRequestMember> RegistrationRequestMembers { get; set; } = new List<RegistrationRequestMember>();
        public virtual ICollection<CourseMember> CourseMembers { get; set; } = new List<CourseMember>();
        public virtual ICollection<CancellationRequest>? CancellationRequests { get; set; }
        public virtual ICollection<StudentNotification>? StudentNotifications { get; set; }

    }
}