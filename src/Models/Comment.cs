namespace src.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public long CreateAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
        public string Text { get; set; } = string.Empty;
    }
}