using src.Models;

namespace src.Utils
{
    public interface ITokenManager
    {
        public Task<string> CreateToken(User user);
        public void hello();
        public User VerifyToken(string token);
    }
}