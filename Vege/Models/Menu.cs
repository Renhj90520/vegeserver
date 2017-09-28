using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Menu
    {
        public int Id { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(70)]
        public string Picture { get; set; }
        [MaxLength(1)]
        public string State { get; set; }
        public ICollection<Recipes> Recipes { get; set; }
        public ICollection<Step> Steps { get; set; }
    }
}
