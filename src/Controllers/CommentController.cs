using src.Data;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.Filters;
using src.Dtos.comment;

using One = src.Dtos.ResponseDto<src.Dtos.comment.DataComment>;
using Many = src.Dtos.ResponseDto<src.Dtos.comment.DataComments>;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IResponseGetter<DataComment> _responseGetterOne;
        private readonly IResponseGetter<DataComments> _responseGetterMany;

        public CommentController(DataContext context, IResponseGetter<DataComment> responseGetterSingle, IResponseGetter<DataComments> responseGetterMany)
        {
            _context = context;
            _responseGetterOne = responseGetterSingle;
            _responseGetterMany = responseGetterMany;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<Many>> GET(int? videoId)
        {
            var comments =
                videoId == null ?
                await _context.Comments
                .Include(c => c.User)
                .Include(c => c.CommentLikes)
                .OrderByDescending(c => c.CommentLikes.Count())
                .ToListAsync()
                :
                await _context.Comments
                .Where(c => c.VideoId == videoId)
                .Include(c => c.User)
                .Include(c => c.CommentLikes)
                .OrderByDescending(c => c.CommentLikes.Count())
                .ToListAsync();

            var user = HttpContext.Items["user"] as User;

            comments.ForEach(c =>
            {
                c.CommentLikes.ForEach(like =>
                {
                    if (like.UserId == user!.Id)
                    {
                        c.IsLiked = true;
                        return;
                    }
                });
            });

            return Ok(_responseGetterMany.Success(new DataComments
            {
                comments = comments
            }));
        }

        [HttpGet("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> GET(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(_responseGetterOne.Error("Comment not found!"));
            }

            return Ok(_responseGetterOne.Success(new DataComment
            {
                comment = comment
            }));
        }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> POST([FromBody] CreateCommentDto input)
        {
            if (input.text == string.Empty)
            {
                return BadRequest(_responseGetterOne.Error("Please input comment text!"));
            }

            var video = await _context.Videos.FindAsync(input.videoId);
            if (video == null)
            {
                return BadRequest(_responseGetterOne.Error("Invalid video Id"));
            }

            var user = HttpContext.Items["user"] as User;

            var comment = new Comment
            {
                VideoId = input.videoId,
                Text = input.text,
                UserId = user!.Id
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterOne.Success(new DataComment
            {
                comment = comment
            }));
        }

        [HttpPut("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> PUT(int id, [FromBody] UpdateCommentDto input)
        {
            if (input.text == string.Empty)
            {
                return BadRequest(_responseGetterOne.Error("Please input comment text!"));
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(_responseGetterOne.Error("Comment not found!"));
            }

            var user = HttpContext.Items["user"] as User;
            if (comment.UserId != user!.Id)
            {
                return Unauthorized(_responseGetterOne.Error("Unauthorized to edit this comment!"));
            }
            comment.Text = input.text;
            await _context.SaveChangesAsync();

            return Ok(_responseGetterOne.Success(new DataComment
            {
                comment = comment
            }));
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<One>> DELETE(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(_responseGetterOne.Error("Comment not found!"));
            }

            var user = HttpContext.Items["user"] as User;
            if (comment.UserId != user!.Id)
            {
                return Unauthorized(_responseGetterOne.Error("Unauthorized to delete this comment!"));
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(_responseGetterOne.Success(new DataComment
            {
                comment = comment
            }));
        }
    }
}
