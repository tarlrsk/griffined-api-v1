using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace griffined_api.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = String.Empty;
        public bool Success { get; set; } = true;
    }
}