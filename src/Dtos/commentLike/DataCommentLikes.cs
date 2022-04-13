using src.Models;

namespace src.Dtos.commentLike
{
    public class DataCommentLikes
    {
        public List<CommentLike> likes { get; set; } = new List<CommentLike>();
    }
}