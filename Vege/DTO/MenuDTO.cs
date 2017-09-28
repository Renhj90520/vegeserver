using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vege.Models;

namespace Vege.DTO
{
    public class MenuDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string State { get; set; }
        public ICollection<RecipesDTO> Recipes { get; set; }
        public ICollection<Step> Steps { get; set; }
    }
}
