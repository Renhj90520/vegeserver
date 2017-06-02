using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double TotalCount { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public double Step { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int State { get; set; } = 1;
        public ICollection<Picture> Pictures { get; set; }
        public int CategoryId { get; set; }
    }
}
