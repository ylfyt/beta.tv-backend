using src.Models;

namespace src.Interfaces
{
    public interface ITokenManager
    {
        public Task<string> CreateToken(User user);
        public void hello();
        public Task<User> VerifyToken(string token);
    }
}