using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Student
    {
        public int id { get; set; }
        public string firebaseId { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        public string nickname { get; set; } = string.Empty;
        public string? profilePicture { get; set; }
        private DateTime _dob;
        public string dob { get { return _dob.ToString("dd-MMMM-yyyy HH:mm:ss"); } set { _dob = DateTime.Parse(value); } }
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
        public string phone { get; set; } = string.Empty;
        public string? line { get; set; }
        public string? email { get; set; }
        public string? school { get; set; }
        public string? countryOfSchool { get; set; }
        public string? levelOfStudy { get; set; }
        public string? program { get; set; }
        public string? targetUni { get; set; }
        public string? targetScore { get; set; }
        public string? hogInfo { get; set; }
        public string? healthInfo { get; set; }
        public _StudentStatusEnum status { get; set; }
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime dateCreated { get; set; } = DateTime.Now;
        public Parent? parent { get; set; }
        public Address? address { get; set; }
        public ICollection<StudentAdditionalFiles>? additionalFiles { get; set; }

    }
}