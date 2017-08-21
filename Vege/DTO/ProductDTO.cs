using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.Models;

namespace Vege.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public double Step { get; set; }
        public double Price { get; set; }
        public ICollection<Picture> Pictures { get; set; }
        public int CategoryId { get; set; }
        public int Sequence { get; set; }
        public int State { get; set; }
    }
}
