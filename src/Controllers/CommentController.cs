using src.Data;
using src.Models;
using Microsoft.AspNetCore.Mvc;
using src.Filters;
using src.Dtos;
using src.Dtos.comment;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly DataContext _context;
        public CommentController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataComments>>> GET(int? videoId)
        {
            var comments =
                videoId == null ?
                await _context.Comments
                .Include(c => c.User)
                .Include(c => c.CommentLikes)
                .ToListAsync()
                :
                await _context.Comments
                .Where(c => c.VideoId == videoId)
                .Include(c => c.User)
                .Include(c => c.CommentLikes)
                .ToListAsync();

            return new ResponseDto<DataComments>
            {
                success = true,
                data = new DataComments
                {
                    comments = comments
                }
            };
        }

        [HttpGet("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataComment>>> GET(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new ResponseDto<DataComment>
                {
                    message = "Comment not found!"
                });
            }

            return new ResponseDto<DataComment>
            {
                success = true,
                data = new DataComment
                {
                    comment = comment
                }
            };
        }

        [HttpPost]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataComment>>> POST([FromBody] CreateCommentDto input)
        {
            if (input.text == string.Empty)
            {
                return BadRequest(new ResponseDto<DataComment>
                {
                    message = "Please input comment text!"
                });
            }

            var video = await _context.Videos.FindAsync(input.videoId);
            if (video == null)
            {
                return BadRequest(new ResponseDto<DataComment>
                {
                    message = "Invalid video Id"
                });
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

            return new ResponseDto<DataComment>
            {
                success = true,
                data = new DataComment
                {
                    comment = comment
                }
            };
        }

        [HttpPut("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataComment>>> PUT(int id, [FromBody] UpdateCommentDto input)
        {
            if (input.text == string.Empty)
            {
                return BadRequest(new ResponseDto<DataComment>
                {
                    message = "Please input comment text!"
                });
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new ResponseDto<DataComment>
                {
                    message = "Comment not found!"
                });
            }

            var user = HttpContext.Items["user"] as User;
            if (comment.UserId != user!.Id)
            {
                return Unauthorized(new ResponseDto<DataComment>
                {
                    message = "Unauthorized to edit this comment!"
                });
            }
            comment.Text = input.text;
            await _context.SaveChangesAsync();

            return new ResponseDto<DataComment>
            {
                success = true,
                data = new DataComment
                {
                    comment = comment
                }
            };
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataComment>>> DELETE(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(new ResponseDto<DataComment>
                {
                    message = "Comment not found!"
                });
            }

            var user = HttpContext.Items["user"] as User;
            if (comment.UserId != user!.Id)
            {
                return Unauthorized(new ResponseDto<DataComment>
                {
                    message = "Unauthorized to delete this comment!"
                });
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return new ResponseDto<DataComment>
            {
                success = true,
                data = new DataComment
                {
                    comment = comment
                }
            };
        }
    }
}
