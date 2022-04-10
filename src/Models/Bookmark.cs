using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public int Id_Video { get; set; }
        public int Id_User { get; set; }
    }
}
