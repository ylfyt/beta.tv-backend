using src.Data;
using src.Models;
using src.Dtos;
using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Filters;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenManager _tm;
        public UserController(ITokenManager tm, DataContext context)
        {
            _context = context;
            _tm = tm;
        }

        [HttpGet("me")]
        [AuthorizationCheckFilter]
        public ActionResult<User> me()
        {
            var user = HttpContext.Items["user"] as User;
            if (user == null)
                return BadRequest();
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> login([FromBody] LoginInputDto input)
        {
            var users = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (users.Count != 1)
                return BadRequest();

            // TODO: Verify HashedPassword
            if (users[0].Password != input.Password)
                return BadRequest();

            string token = await _tm.CreateToken(users[0]);

            Response.Headers.Add("Authorization", token);

            return users[0];
        }
    }
}
