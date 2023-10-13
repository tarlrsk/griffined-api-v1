using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    [Index(nameof(Type))]
    public class StudyCourseHistory
    {
        public int Id { get; set; }
        public int? StudyCourseId { get; set; }
        public int? StaffId { get; set; }
        public int? StudyClassId { get; set; }
        public int? StudentId { get; set; }

        public string Description { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
        public StudyCourseHistoryType Type { get; set; }
        public StudyCourseHistoryMethod Method { get; set; }

        [ForeignKey(nameof(StudyClassId))]
        public StudyClass? StudyClass { get; set; }
        
        [ForeignKey(nameof(StudentId))]
        public Student? Student { get; set; }

        [ForeignKey(nameof(StudyCourseId))]
        public virtual StudyCourse StudyCourse { get; set; } = new StudyCourse();

        [ForeignKey(nameof(StaffId))]
        public virtual Staff Staff { get; set; } = new Staff();
    }
}