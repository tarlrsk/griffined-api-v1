using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class Staff
    {
        public int id { get; set; }
        public string firebaseId { get; set; } = string.Empty;
        public string fName { get; set; } = string.Empty;
        public string lName { get; set; } = string.Empty;
        public string fullName { get { return fName + " " + lName; } }
        public string nickname { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string line { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public int? LastUpdatedBy { get; set; }
        public ICollection<StaffNotification> staffNotifications { get; set; } = new List<StaffNotification>();
        public ICollection<StudyCourseHistory> studyCourseHistories { get; set; } = new List<StudyCourseHistory>();
    }
}