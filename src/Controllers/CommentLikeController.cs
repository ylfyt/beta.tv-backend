using src.Data;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.Filters;
using src.Dtos.commentLike;

using One = src.Dtos.ResponseDto<src.Dtos.commentLike.DataCommentLike>;
using Many = src.Dtos.ResponseDto<src.Dtos.commentLike.DataCommentLikes>;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentLikeController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IResponseGetter<DataCommentLike> _responseGetterSingle;
        private readonly IResponseGetter<DataCommentLikes> _responseGetterMany;
        public CommentLikeController(DataContext context, IResponseGetter<DataCommentLike> responseGetterSingle, IResponseGetter<DataCommentLikes> responseGetterMany)
        {
            _context = context;
            _responseGetterSingle = responseGetterSingle;
            _responseGetterMany = responseGetterMany;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GET(int commentId)
        {
            return Ok(_responseGetterMany.Success(new DataCommentLikes
            {
                likes = await _context.CommentLikes.Where(l => l.CommentId == commentId).Include(l => l.Comment).ToListAsync()
            }));
        }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> POST([FromBody] CreateLikeDto input)
        {
            var comment = await _context.Comments.FindAsync(input.commentId);
            if (comment == null)
            {
                return NotFound(_responseGetterSingle.Error("Comment Not Found"));
            }
            var user = HttpContext.Items["user"] as User;
            var tempLike = await _context.CommentLikes.Where(l => l.UserId == user!.Id && l.CommentId == input.commentId).ToListAsync();

            if (tempLike.Count != 0)
            {
                return BadRequest(_responseGetterSingle.Error("Already liked"));
            }

            var like = new CommentLike
            {
                CommentId = comment.Id,
                UserId = user!.Id,
            };

            _context.Add(like);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataCommentLike
            {
                like = like
            }));
        }

        [HttpDelete]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> DELETE(int commentId)
        {
            var user = HttpContext.Items["user"] as User;
            var likes = await _context.CommentLikes.Where(l => l.CommentId == commentId && l.UserId == user!.Id).ToListAsync();

            if (likes.Count == 0)
            {
                return NotFound(_responseGetterSingle.Error("Like Not Found"));
            }

            _context.Remove(likes[0]);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterSingle.Success(new DataCommentLike
            {
                like = likes[0]
            }));
        }
    }
}
