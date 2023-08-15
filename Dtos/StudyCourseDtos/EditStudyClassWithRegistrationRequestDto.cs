using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.StudyCourseDtos
{
    public class EditStudyClassByRegistrationRequestDto
    {
        public List<int> ClassToDelete { get; set; } = new List<int>();
        public List<NewStudyClassScheduleRequestDto> ClassToAdd { get; set; } = new List<NewStudyClassScheduleRequestDto>();
    }
}