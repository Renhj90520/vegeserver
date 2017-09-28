using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.Models;

namespace Vege.DTO
{
    public class RecipesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public double Step { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
    }
}
