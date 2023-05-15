using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Teacher
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string firebaseId { get; set; } = string.Empty;
        [Required]
        public string fName { get; set; } = string.Empty;
        [Required]
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        [Required]
        public string nickname { get; set; } = string.Empty;
        [Required]
        public string phone { get; set; } = string.Empty;
        [Required]
        public string email { get; set; } = string.Empty;
        [Required]
        public string line { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        [Required]
        public ICollection<WorkTime> workTimes { get; set; } = new List<WorkTime>();
        public ICollection<TeacherPrivateClass>? privateClasses { get; set; }
        public ICollection<TeacherLeavingRequest>? leavingRequests { get; set; }
    }
}