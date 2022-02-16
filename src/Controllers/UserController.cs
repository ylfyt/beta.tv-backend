using src.Data;
using src.Models;
using src.Dtos;
using Microsoft.AspNetCore.Mvc;
using src.Utils;

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
        public async Task<ActionResult<User>?> me()
        {
            User user = new User
            {
                Id = 1,
                Name = "Yudi",
                Username = "yudi",
                Email = "yudi@gmail.com",
                Password = "1234"
            };

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

            var logs = await _context.TokenLogs.Where(x => x.UserId == users[0].Id).ToListAsync();
            TokenLog log;

            if (logs.Count != 1)
            {
                string token = _tm.CreateToken(users[0]);
                log = new TokenLog
                {
                    UserId = users[0].Id,
                    Token = token,
                    Status = true,
                };
                await _context.TokenLogs.AddAsync(log);
            }
            else
            {
                log = logs[0];
                if (!log.Status)
                    log.Status = true;
            }
            await _context.SaveChangesAsync();

            Response.Headers.Add("Authentication", log.Token);

            return users[0];
        }
    }
}
