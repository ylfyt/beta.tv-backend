using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace src.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Level { get; set; } = UserLevel.USER;
        public string Email { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; } = false;
        [JsonIgnore]
        public string PasswordSalt { get; set; } = string.Empty;
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
        public string PhotoURL { get; set; } = string.Empty;
    }
}