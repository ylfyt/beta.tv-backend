namespace src.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
