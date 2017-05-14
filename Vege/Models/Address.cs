using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Address
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }

    }
}
