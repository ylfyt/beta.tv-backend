using System.ComponentModel.DataAnnotations;
namespace src.Dtos.video
{
    public class VideoAddDto
    {
        //public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string AuthorTitle { get; set; } = string.Empty;
        [Required]
        [StringLength(10000),MinLength(10)]
        public string AuthorDescription { get; set; } = string.Empty;
        public string YoutubeVideoId { get; set; } = string.Empty;
        public List<string> CategorySlugs { get; set; } = null!;
    }
}
