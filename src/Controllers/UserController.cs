using src.Data;
using src.Models;
using src.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
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
        public async Task<ActionResult<LoginResponse>> login([FromBody] LoginInputDto input)
        {
            var users = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (users.Count != 1)
            {
                return BadRequest();
            }

            if (users[0].Password != input.Password)
            {
                return BadRequest();
            }

            var res = new LoginResponse
            {
                Id = users[0].Id,
                Username = users[0].Username,
                Token = "dsaihdlkasdlkasndjklasnkmdkqoeq21"
            };

            return res;
        }
    }
}
