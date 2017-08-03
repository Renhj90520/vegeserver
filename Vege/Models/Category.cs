using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Category
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(10)]
        public string Name { get; set; }
        [MaxLength(70)]
        public string IconPath { get; set; }
        [MaxLength(1)]
        public string State { get; set; } = "1";
        public int Sequential { get; set; } = 0;
    }
}
