using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Step
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
        [MaxLength(300)]
        public string Description { get; set; }
    }
}
