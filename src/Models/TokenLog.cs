using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    public class TokenLog
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}