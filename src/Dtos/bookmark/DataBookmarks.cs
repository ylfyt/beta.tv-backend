using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Models;

namespace src.Dtos.bookmark
{
    public class DataBookmarks
    {
        public List<Bookmark> bookmark { get; set; } = new List<Bookmark>();
    }
}

