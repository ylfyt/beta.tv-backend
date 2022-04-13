using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Video")]
        [Column(Order = 2)]
        public int VideoId { get; set; }
        [ForeignKey("User")]
        [Column(Order = 3)]
        public int UserId { get; set; }
    }
}
