using System.ComponentModel.DataAnnotations;

namespace TwitterWebApp1.Models.Home
{
    public class CreatePostBoxViewModel
    {
        public string? ProfileImage { get; set; }

        [Required]
        public string? Description { get; set; }

        public IFormFile? InputImageFile { get; set; }

        // public string PostImage { get; set; }
    }

}