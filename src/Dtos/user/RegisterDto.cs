using System.ComponentModel.DataAnnotations;

namespace src.Dtos
{
    public class RegisterDto
    {
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [StringLength(50),MinLength(5)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [StringLength(100),MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}