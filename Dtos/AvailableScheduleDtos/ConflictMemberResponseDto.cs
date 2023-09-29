using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace griffined_api.Dtos.AvailableScheduleDtos
{
    public class ConflictMemberResponseDto
    {
        public string Role { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
    }
}