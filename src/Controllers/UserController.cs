using src.Data;
using src.Models;
using src.Dtos;
using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Filters;

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using src.Dtos.user;

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
        public async Task<ActionResult<ResponseDto<DataUser>>> me()
        {
            var userAuth = HttpContext.Items["user"] as User;

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = userAuth!
                }
            });
        }
        
        [HttpGet]
        public async Task<ActionResult<ResponseDto<DataUser>>> GetAllUser()
        {
            List<User> allUser = await _context.User.ToListAsync();
            var response = new ResponseDto<DataUsers>
            {
                success = true,
                data = new DataUsers
                {
                    users = allUser
                }
            };
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto<DataUser>>> login([FromBody] LoginInputDto input)
        {
            var users = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (users.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            if (!VerifyPassword(input.Password, users[0]))
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            string token = await _tm.CreateToken(users[0]);

            Response.Headers.Add("Authorization", token);

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = users[0]
                }
            });
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
        public async Task<ActionResult<ResponseDto<DataUser>>> logout()
        {
            var user = HttpContext.Items["user"] as User;
            var logs = await _context.TokenLogs.Where(x => x.UserId == user!.Id).ToListAsync();
            if (logs.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            logs[0].Status = false;
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto<DataUser>>> register([FromBody] RegisterDto input)
        {
            if (input.Username.Length < 3 || input.Password.Length < 3 || input.Email == "" || input.Name == "")
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            var users = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (users.Count != 0)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            string password = input.Password;

            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            var insert = new User
            {
                Name = input.Name,
                Username = input.Username,
                Email = input.Email,
                PasswordSalt = Convert.ToBase64String(salt),
                Password = hashed
            };

            await _context.User.AddAsync(insert);
            await _context.SaveChangesAsync();

            string token = await _tm.CreateToken(insert);

            Response.Headers.Add("Authorization", token);

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = insert
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<DataUser>>> DeleteUser(int id)
        {
            var deletedUser = await _context.User.Where(v => v.Id == id).ToListAsync();
            if (deletedUser.Count != 1)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "User doesn't exist"
                });
            }
            _context.User.Remove(deletedUser[0]);
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = deletedUser[0]
                }
            });
        }
    }
}
