using System.ComponentModel.DataAnnotations;
namespace src.Dtos.video
{
    public class VideoUpdateDto
    {
        [Required]
        public string AuthorTitle { get; set; } = string.Empty;

        public List<string> CategorySlugs { get; set; } = null!;
        [Required]
        [StringLength(10000),MinLength(5)]
        public string AuthorDescription { get; set; } = string.Empty;
    }
}
