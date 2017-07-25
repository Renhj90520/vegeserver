using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class RefundWrapper
    {
        public int TotalCost { get; set; }
        public int RefundCost { get; set; }
        public string RefundNote { get; set; }
    }
}
