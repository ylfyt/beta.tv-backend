namespace src.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ChannelId { get; set; }
        public string Url { get; set; } = string.Empty;
        public int Views { get; set; }
        public float Rating { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Release_Date { get; set; }
    }
}
