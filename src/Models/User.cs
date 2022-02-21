using System.Text.Json.Serialization;

namespace src.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [JsonIgnore]
        public string PasswordSalt { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
    }
}