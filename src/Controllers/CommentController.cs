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
        public async Task<ActionResult<ResponseDto<DataComments>>> GET()
        {
            var comments = await _context.Comments.ToListAsync();
            return new ResponseDto<DataComments>
            {
                success = true,
                data = new DataComments
                {
                    comments = comments
                }
            };
        }
    }
}
