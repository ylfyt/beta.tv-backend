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

        [HttpGet]
        [AuthorizationCheckFilter(UserLevel.ADMIN)]
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

            BuildEmailTemplate(insert.Id, insert.Email, eToken);

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
            var userLog = await _context.EmailTokenLogs.Where(x => x.Token == eToken).ToListAsync();
            if (userLog.Count != 1)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "Token Not Found"
                });
            }
            var user = await _context.User.FindAsync(userLog[0].UserId);
            if (user == null)
            {
                return NotFound(new ResponseDto<DataUser>
                {
                    message = "User Not Found"
                });
            }
            user.IsConfirmed = true;
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

        /*public System.Web.Mvc.ActionResult Confirm(int id)
        {
            ViewBag.Id = id;
            var user = _context.User.Where(x => x.Id == id).FirstOrDefaultAsync();
            user.IsConfirmed = true;
            _context.SaveChangesAsync();
            return View();
        }*/

        /*public Microsoft.AspNetCore.Mvc.JsonResult RegisterConfirm(int id)
        {
            var user = _context.User.Where(x => x.Id == id).FirstOrDefaultAsync();
            user.IsConfirmed = true;
            _context.SaveChangesAsync();
            var msg = "Your Email is verified!";
            return System.Web.Mvc.JsonResult(new { });
        }*/

        private async void BuildEmailTemplate(int id, string email, string eToken)
        {
            string body = "<html><head></head><body><div><a href=\"@ViewBag.ConfirmationLink\">Here</a></div></body></html>";
            try
            {
                body = System.IO.File.ReadAllText("../if3250_2022_buletin_backend/src/EmailTemplate/Text.cshtml");
            }
            catch
            {
            }
            var regInfo = email;
            var url = "http://localhost:3000/" + "confirm/" + eToken;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account is Successfully Created", body, regInfo);
        }

        private void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "ignatiusdavidpartogi@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential(Credentials.SENDER_EMAIL, Credentials.SENDER_PASS);
            try
            {
                client.Send(mail);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("Error in SendEmail: " + ex.Message);
            }
        }
    }
}
