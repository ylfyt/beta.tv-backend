namespace src.Models
{
    public class History
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        public DateTime Access_Video { get; set; }
    }
}
