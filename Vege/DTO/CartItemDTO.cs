using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.Models;

namespace Vege.DTO
{
    public class CartItemDTO : Product
    {
        public int Count { get; set; }
    }
}
