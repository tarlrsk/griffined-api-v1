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
        public int Id { get; set; }

        public string StudentCode { get; set; } = string.Empty;
        public string FirebaseId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get { return FirstName + " " + LastName; } }
        public string Nickname { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public int Age
        {
            get
            {
                int _age = 0;
                _age = DateTime.Now.Subtract(DOB).Days;
                _age /= 365;
                return _age;
            }
        }
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

        public StudentStatus Status { get; set; } = StudentStatus.Inactive;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; } = new DateTime(2030, 12, 31);

        public virtual ProfilePicture? ProfilePicture { get; set; }
        public virtual Parent? Parent { get; set; }
        public virtual Address? Address { get; set; }
        public virtual StudentAttendance? Attendance { get; set; }

        public virtual ICollection<StudentAdditionalFile>? AdditionalFiles { get; set; }
        public virtual ICollection<RegistrationRequestMember> RegistrationRequestMembers { get; set; } = new List<RegistrationRequestMember>();
        public virtual ICollection<StudySubjectMember> StudySubjectMember { get; set; } = new List<StudySubjectMember>();
        public virtual ICollection<CancellationRequest>? CancellationRequests { get; set; }
        public virtual ICollection<StudentNotification>? StudentNotifications { get; set; }

    }
}