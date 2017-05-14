using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Result<T>
    {
        public string Message { get; set; }
        public int State { get; set; }
        public T Body { get; set; }
    }
}
