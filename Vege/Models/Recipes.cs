using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Recipes
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int ProductId { get; set; }
        [MaxLength(20)]
        public string ProductName { get; set; }
    }
}
