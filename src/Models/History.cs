using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        [Column(Order = 1)]
        public int UserId { get; set; }
        [ForeignKey("Video")]
        [Column(Order = 1)]
        public int VideoId { get; set; }
        public DateTime Access_Video { get; set; }
    }
}
