using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.DTO
{
    public class CartItemDTO : ProductDTO
    {
        public int Count { get; set; }
    }
}
