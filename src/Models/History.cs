using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace if3250_2022_01_buletin_backend.src.Models
{
    public class History
    {
        public int ID_User { get; set; }
        public int ID_Video { get; set; }
        public DateTime Access_Video { get; set; }
    }
}
