using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string OpenId { get; set; }
    }
}
