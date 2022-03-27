using src.Models;

namespace src.Dtos.user
{
    public class DataUser
    {
        public User user { get; set; } = null!;
        public string token { get; set; } = string.Empty;
    }
}