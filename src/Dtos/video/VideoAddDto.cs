namespace src.Dtos.video
{
    public class VideoAddDto
    {
        //public int Id { get; set; }
        public string AuthorTitle { get; set; } = string.Empty;
        public string AuthorDescription { get; set; } = string.Empty;
        public string YoutubeVideoId { get; set; } = string.Empty;
        public List<string> CategorySlugs { get; set; } = null!;
    }
}
