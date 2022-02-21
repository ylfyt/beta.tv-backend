using src.Data;
using src.Models;
using src.Dtos;
using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Filters;

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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

            if (!VerifyPassword(input.Password, users[0]))
                return BadRequest();

            string token = await _tm.CreateToken(users[0]);

            Response.Headers.Add("Authorization", token);

            return users[0];
        }

        private bool VerifyPassword(string password, User user)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(user.PasswordSalt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed == user.Password;
        }

        [HttpPost("logout")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<bool>> logout()
        {
            var user = HttpContext.Items["user"] as User;
            var logs = await _context.TokenLogs.Where(x => x.UserId == user!.Id).ToListAsync();
            if(logs.Count != 1) {
                return false;
            }
            Console.WriteLine("sampe sini");

            logs[0].Status = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
