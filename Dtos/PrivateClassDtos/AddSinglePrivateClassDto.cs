using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateClassDtos
{
    public class AddSinglePrivateClassDto
    {
        public int courseId { get; set; }
        public AddPrivateClassDto privateClass { get; set; } = new AddPrivateClassDto();
    }
}