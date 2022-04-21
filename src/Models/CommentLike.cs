using System.Text.Json.Serialization;

namespace src.Models
{
    public class CommentLike
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int CommentId { get; set; }
        [JsonIgnore]
        public Comment? Comment { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        public long CreateAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}