using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Result<T>
    {
        public string message { get; set; }
        public int state { get; set; }
        public T body { get; set; }
    }
}
