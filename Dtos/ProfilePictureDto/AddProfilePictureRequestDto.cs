using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ProfilePictureDto
{
    public class AddProfilePictureRequestDto
    {
        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public required IFormFile PictureData { get; set; }
    }
}