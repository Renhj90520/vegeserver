using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int isPrimary { get; set; } = 0;
    }
}
