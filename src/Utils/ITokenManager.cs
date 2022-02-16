using src.Models;

namespace src.Utils
{
    public interface ITokenManager
    {
        public string CreateToken(User user);
    }
}