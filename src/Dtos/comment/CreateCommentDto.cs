namespace src.Dtos.comment
{
    public class CreateCommentDto
    {
        public int videoId { get; set; }
        public string text { get; set; } = string.Empty;
    }
}
