using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class ItemsResult<T>
    {
        public int Count { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
