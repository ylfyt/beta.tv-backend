namespace src.Dtos.video
{
    public class VideoUpdateDto
    {
        public string AuthorTitle { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new List<string>();
        public string AuthorDescription { get; set; } = string.Empty;
    }
}
