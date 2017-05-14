using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public ICollection<CartItem> Products { get; set; }
        public string OpenId{ get; set; }
    }
}
