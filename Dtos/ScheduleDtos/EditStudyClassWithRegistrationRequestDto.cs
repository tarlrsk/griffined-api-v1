using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ScheduleDtos
{
    public class EditStudyClassByRegistrationRequestDto
    {
        public List<int> ClassToDelete = new List<int>();
        public List<NewStudyClassScheduleRequestDto> ClassToAdd = new List<NewStudyClassScheduleRequestDto>();
    }
}