using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace if3250_2022_01_buletin_backend.src.Models
{
    public class Channel
    {
        public int ID_Channel { get; set; }
        public string Channel_Owner { get; set; }
        public int Subscriber_Count { get; set; }
    }
}
