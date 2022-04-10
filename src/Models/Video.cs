using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        public string YoutubeVideoId { get; set; } = string.Empty;
        [Required]
        [MaxLength(50, ErrorMessage = "Title of the Video cannot be more than 50 characters!")]
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ChannelThumbnailUrl { get; set; } = string.Empty;
        [ForeignKey("Channel")]
        [Column(Order=1)]
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
        public string Description { get; set; } = string.Empty;
        public string CreateAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        [Required]
        [MaxLength(1000, ErrorMessage = "Description of the Author cannot be more than 50 characters!")]
        public string AuthorDescription { get; set; } = string.Empty;
        public string AuthorTitle { get; set; } = string.Empty;
        [ForeignKey("User")]
        [Column(Order=2)]
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }
}
