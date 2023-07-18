using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace griffined_api.Dtos.ProfilePictureDto
{
    public class UpdateProfilePictureRequestDto
    {
        [Required]
        public string FileName { get; set; } = String.Empty;

        [Required]
        public required IFormFile PictureData { get; set; }
    }
}