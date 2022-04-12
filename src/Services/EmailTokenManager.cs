using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using src.Models;
using src.Data;

namespace src.Interfaces
{
    public class EmailTokenManager : ITokenManager
    {
        private readonly DataContext _context;
        public static int count { get; set; } = 0;
        public EmailTokenManager(DataContext context)
        {
            _context = context;
            count++;
        }
        public async Task<string> CreateToken(User user)
        {
            var logs = await _context.TokenLogs.Where(x => x.UserId == user.Id).ToListAsync();
            TokenLog log;

            if (logs.Count != 1)
            {
                List<Claim> claims = new List<Claim>{
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email)
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Constants.JWT_SECRET_KEY));
                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var jwt = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: cred
                );

                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                log = new TokenLog
                {
                    UserId = user.Id,
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

            return log.Token;
        }

        public void hello()
        {
            // Console.WriteLine(_context.Products.Find(1)?.Name);
            Console.WriteLine("Hello TM");
        }

        public async Task<User> VerifyToken(string token)
        {
            var claims = new JwtSecurityTokenHandler().ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Constants.JWT_SECRET_KEY)),
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken
            );

            if (claims == null)
                throw new Exception("Not Authorized");


            var userIdClaim = claims.Claims.ToList().Find(x => x.Type == "id");
            var emailClaim = claims.Claims.ToList().Find(x => x.Type == "email");
            if (userIdClaim == null || emailClaim == null)
                throw new Exception("Not Authorized");

            int userId = Int32.Parse(userIdClaim.Value);
            string userEmail = emailClaim.Value;

            var logs = await _context.TokenLogs.Where(x => x.UserId == userId).ToListAsync();
            if (logs.Count != 1)
                throw new Exception("Not Authorized");

            if (!logs[0].Status)
                throw new Exception("Not Authorized");

            var user = await _context.User.FindAsync(userId);

            if (user == null)
                throw new Exception("Not Authorized");

            return user;
        }
    }
}
