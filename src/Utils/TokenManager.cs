using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using src.Models;

namespace src.Utils
{
    public class TokenManager : ITokenManager
    {
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>{
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username)
        };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("secret key dkasjlkdasjmkldasmkdamslkdmaskmd"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var jwt = new JwtSecurityToken(
                claims: claims,
                signingCredentials: cred
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }
    }
}