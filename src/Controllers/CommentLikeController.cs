using src.Data;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.Filters;
using src.Dtos;
using src.Dtos.comment;
using src.Dtos.commentLike;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentLikeController : ControllerBase
    {
        private readonly DataContext _context;
        public CommentLikeController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataCommentLikes>>> GET(int commentId)
        {
            return new ResponseDto<DataCommentLikes>
            {
                success = true,
                data = new DataCommentLikes
                {
                    likes = await _context.CommentLikes.Where(l => l.CommentId == commentId).Include(l => l.Comment).ToListAsync()
                }
            };
        }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataCommentLike>>> POST([FromBody] CreateLikeDto input)
        {
            var comment = await _context.Comments.FindAsync(input.commentId);
            if (comment == null)
            {
                return NotFound(new ResponseDto<DataCommentLike>
                {
                    message = "Comment Not Found"
                });
            }
            var user = HttpContext.Items["user"] as User;
            var tempLike = await _context.CommentLikes.Where(l => l.UserId == user!.Id && l.CommentId == input.commentId).ToListAsync();

            if (tempLike.Count != 0)
            {
                return BadRequest(new ResponseDto<DataCommentLike>
                {
                    message = "Already liked"
                });
            }

            var like = new CommentLike
            {
                CommentId = comment.Id,
                UserId = user!.Id,
            };

            _context.Add(like);
            await _context.SaveChangesAsync();

            return new ResponseDto<DataCommentLike>
            {
                success = true,
                data = new DataCommentLike
                {
                    like = like
                }
            };
        }

        [HttpDelete]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataCommentLike>>> DELETE(int commentId)
        {
            var user = HttpContext.Items["user"] as User;
            var likes = await _context.CommentLikes.Where(l => l.CommentId == commentId && l.UserId == user.Id).ToListAsync();

            if (likes.Count == 0)
            {
                return BadRequest(new ResponseDto<DataCommentLike>
                {
                    message = "Like Not Found"
                });
            }

            _context.Remove(likes[0]);
            await _context.SaveChangesAsync();

            return new ResponseDto<DataCommentLike>
            {
                success = true,
                data = new DataCommentLike
                {
                    like = likes[0]
                }
            };
        }
    }
}
