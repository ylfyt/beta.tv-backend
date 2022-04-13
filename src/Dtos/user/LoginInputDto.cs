using System.ComponentModel.DataAnnotations;
namespace src.Dtos
{
    public class LoginInputDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}