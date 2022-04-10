using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Channel")]
        [Column(Order =1)]
        public int ChannelId { get; set; }
        [ForeignKey("Video")]
        [Column(Order = 2)]
        public int Id_Video { get; set; }
        [ForeignKey("User")]
        [Column(Order = 3)]
        public int Id_User { get; set; }
    }
}
