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
using System.Text;
using System.Net.Mail;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenManager _tm;
        private readonly IEmailTokenManager _etm;
        public UserController(ITokenManager tm, IEmailTokenManager etm, DataContext context)
        {
            _context = context;
            _tm = tm;
            _etm = etm;
        }

        [HttpGet("me")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataUser>>> me()
        {
            var userAuth = HttpContext.Items["user"] as User;
            var user = await _context.User.FindAsync(userAuth!.Id);
            if (user == null)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "User Not Found"
                });
            }

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = userAuth!
                }
            });
        }

        [HttpGet("admin/me")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<ResponseDto<DataUser>>> meAdmin()
        {
            var userAuth = HttpContext.Items["user"] as User;
            var user = await _context.User.FindAsync(userAuth!.Id);
            if (user == null)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "User Not Found"
                });
            }

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
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
        public async Task<ActionResult<ResponseDto<DataUsers>>> GetAllUser()
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
                    user = users[0],
                    token = token
                }
            });
        }

        [HttpPost("admin/login")]
        public async Task<ActionResult<ResponseDto<DataUser>>> loginAdmin([FromBody] LoginInputDto input)
        {
            var users = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (users.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Bad request"
                });
            }

            if (users[0].Level != UserLevel.ADMIN)
            {
                return Unauthorized(new ResponseDto<DataUser>
                {
                    message = "Forbidden"
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
                    user = users[0],
                    token = token
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

            string eToken = await _etm.CreateToken(insert);

            SendEmailConfirmation(insert.Id, insert.Email, eToken);

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = insert,
                    token = token
                }
            });
        }

        [HttpPost("confirm/{eToken}")]
        public async Task<ActionResult<ResponseDto<DataUser>>> Confirm(string eToken)
        {
            var log = await _context.EmailTokenLogs.Where(x => x.Token == eToken && x.Status == true).FirstOrDefaultAsync();
            if (log == null)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "Token Not Found"
                });
            }
            var user = await _context.User.FindAsync(log.UserId);
            if (user == null)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "User Not Found"
                });
            }
            user.IsConfirmed = true;
            log.Status = false;
            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = user
                }
            });
        }

        [HttpDelete("{id}")]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
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

        private async void SendEmailConfirmation(int id, string email, string eToken)
        {
            var url = ServerInfo.ADMIN_PAGE_URL + "/confirm/" + eToken;
            string body = $"<h2>Thank you for joining us 😊</h2> <a href='{url}'>Click here to confirm your email.</a>";
            string subject = "Your Account is Successfully Created";

            string from = ServerInfo.EMAIL_ADDRESS;

            MailMessage mail = new MailMessage(from, email);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            await SendEmail(mail);
        }

        private async Task SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(ServerInfo.EMAIL_ADDRESS, ServerInfo.EMAIL_PASSWORD);
            try
            {
                await client.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error in SendEmail: " + ex.Message);
            }
        }

        [HttpPost("changeProfile")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataUser>>> changeProfile([FromBody] ChangeProfileDto input)
        {
            var selectedUser = await _context.User.Where(x => x.Username == input.OldUsername).ToListAsync();

            if (selectedUser.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Invalid account"
                });
            }

            // check password
            if (!VerifyPassword(input.Password, selectedUser[0]))
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Wrong password"
                });
            }

            selectedUser[0].Name = input.Name;
            selectedUser[0].Username = input.Username;
            selectedUser[0].Email = input.Email;

            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = selectedUser[0]
                }
            });
        }

        [HttpPost("changePass")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataUser>>> changePassword([FromBody] ChangePassDto input)
        {
            var selectedUser = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (selectedUser.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = selectedUser.Count.ToString()
                });
            }

            // check password
            if (!VerifyPassword(input.OldPassword, selectedUser[0]))
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Wrong password"
                });
            }

            // change password
            string newpass = input.NewPassword;

            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newpass,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            selectedUser[0].PasswordSalt = Convert.ToBase64String(salt);
            selectedUser[0].Password = hashed;

            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = selectedUser[0]
                }
            });
        }
        /*
        public static IWebHostEnvironment _environment;

        [HttpPost("changeProfilePic")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<string>>> changeProfilePic(IFormFile file){

            try{
                var httpRequest = HttpContext.Current.Request;
                var filename;
                if (httpRequest.Files.Count > 0){
                    foreach (string file in httpRequest.Files){
                        var postedFile = httpRequest.Files[file];
                        fileName = postedFile.FileName.Split('\\').LastOrDefault().Split('/').LastOrDefault();
                        var filePath = HttpContext.Current.Server.MapPath("~/Uploads/"+fileName);
                        postedFile.SaveAs(filePath);
                    }

                    return Ok(new ResponseDto<string>
                    {
                        success = true,
                        data = "/Uploads/"+fileName
                    });
                }
                else{
                    return BadRequest(new ResponseDto<string>
                    {
                        message = "invalid file upload"
                    });
                }
            }
            catch(Exception e){
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = e.Message
                });
            }



        }
        */

        [HttpPost("changeProfilePic")]
        [AuthorizationCheckFilter]
        public async Task<ActionResult<ResponseDto<DataUser>>> changeProfilePhoto([FromBody] ChangeProfPhotoDto input)
        {
            var selectedUser = await _context.User.Where(x => x.Username == input.Username).ToListAsync();

            if (selectedUser.Count != 1)
            {
                return BadRequest(new ResponseDto<DataUser>
                {
                    message = "Invalid account"
                });
            }

            selectedUser[0].PhotoURL = input.PhotoURL;

            await _context.SaveChangesAsync();

            return Ok(new ResponseDto<DataUser>
            {
                success = true,
                data = new DataUser
                {
                    user = selectedUser[0]
                }
            });
        }
    }
}
