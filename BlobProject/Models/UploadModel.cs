using System.ComponentModel.DataAnnotations;

namespace BlobProject.Models
{
    public class UploadModel
    {
        [Required]
        public string Text { get; set; }

        public IFormFile Image { get; set; }
    }
}
