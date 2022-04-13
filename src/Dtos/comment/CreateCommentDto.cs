using System.ComponentModel.DataAnnotations;
namespace src.Dtos.comment
{
    public class CreateCommentDto
    {
        public int videoId { get; set; }
        [StringLength(1000)]
        public string text { get; set; } = string.Empty;
    }
}
