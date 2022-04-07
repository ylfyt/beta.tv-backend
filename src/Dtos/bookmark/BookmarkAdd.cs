using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.Dtos.bookmark
{
    public class BookmarkAdd
    {
        public int ChannelId { get; set; }
        public int Id_Video { get; set; }
        public int Id_User { get; set; }
    }
}
