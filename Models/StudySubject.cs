using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class StudySubject
    {
        public int id { get; set; }
        public int subjectId { get; set; }
        public Subject subject { get; set; } = new Subject();
        public int studyCourseId { get; set; }
        public StudyCourse studyCourse { get; set; } = new StudyCourse();
        public ICollection<CancellationRequest> cancellationRequests { get; set; } = new List<CancellationRequest>();
    }
}