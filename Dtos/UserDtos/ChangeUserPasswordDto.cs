using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.UserDtos
{
    public class ChangeUserPasswordDto
    {
        public string password { get; set; } = string.Empty;
        public string verifyPassword { get; set; } = string.Empty;
    }
}