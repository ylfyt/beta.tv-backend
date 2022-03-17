using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace if3250_2022_01_buletin_backend.src.Models
{
    public class Video
    {
        public int ID_Video { get; set; }
        public string Video_Creator { get; set; }
        public string Link_Video { get; set; }
        public int Views { get; set; }
        public float Rating { get; set; }
        public string Kategori { get; set; }
        public DateTime Release_Date { get; set; }
    }
}
