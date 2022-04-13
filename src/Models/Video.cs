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
        [MaxLength(256, ErrorMessage = "Title of the Video cannot be more than 50 characters!")]
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string ChannelThumbnailUrl { get; set; } = string.Empty;
        [ForeignKey("Channel")]
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new List<Category>();
        public string Description { get; set; } = string.Empty;
        public long CreateAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        [Required]
        [MaxLength(1000, ErrorMessage = "Description of the Author cannot be more than 50 characters!")]
        public string AuthorDescription { get; set; } = string.Empty;
        public string AuthorTitle { get; set; } = string.Empty;
        [ForeignKey("User")]
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }
}
