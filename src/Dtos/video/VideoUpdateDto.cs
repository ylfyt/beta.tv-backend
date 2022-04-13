namespace src.Dtos.video
{
    public class VideoUpdateDto
    {
        public string AuthorTitle { get; set; } = string.Empty;

        public List<string> CategorySlugs { get; set; } = null!;
        public string AuthorDescription { get; set; } = string.Empty;
    }
}
