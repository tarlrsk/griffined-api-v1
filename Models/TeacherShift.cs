using System.ComponentModel.DataAnnotations.Schema;

namespace griffined_api.Models
{
    public class TeacherShift
    {
        public int Id { get; set; }
        public int? TeacherId { get; set; }
        public int? StudyClassId { get; set; }
        public double Hours { get; set; }
        public TeacherWorkType TeacherWorkType { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey(nameof(StudyClassId))]
        public virtual StudyClass? StudyClass { get; set; }
    }

}