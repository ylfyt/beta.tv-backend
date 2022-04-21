using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace src.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [JsonIgnore]
        public List<CommentLike> CommentLikes { get; set; } = new List<CommentLike>();
        [NotMapped]
        public int CountLikes => CommentLikes.Count();
        [NotMapped]
        public bool IsLiked { get; set; } = false;
        public long CreateAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string Text { get; set; } = string.Empty;
    }
}