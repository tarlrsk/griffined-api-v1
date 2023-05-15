using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.PrivateClassDtos
{
    public class GetSinglePrivateClassDto
    {
        public int courseId { get; set; }
        public GetPrivateClassDto privateClass { get; set; } = new GetPrivateClassDto();
    }
}