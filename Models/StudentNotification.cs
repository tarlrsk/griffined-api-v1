using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudentNotification
    {
        public int id { get; set; }
        public int? studentId { get; set; }
        public int? studyCourseId { get; set; }

        public DateTime dateCreated { get; set; } = DateTime.Now;
        public virtual StudentNotificationType type { get; set; }
        public bool hasRead { get; set; }

        [ForeignKey(nameof(studentId))]
        public virtual Student student { get; set; } = new Student();

        [ForeignKey(nameof(studyCourseId))]
        public virtual StudyCourse studyCourse { get; set; } = new StudyCourse();
    }
}