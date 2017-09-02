using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class User
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public string UserName { get; set; }
        [MaxLength(32)]
        public string Password { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(40)]
        public string NickName { get; set; }
        [MaxLength(15)]
        public string Phone { get; set; }
        [MaxLength(28)]
        public string OpenId { get; set; }
        [MaxLength(20)]
        public string Province { get; set; }
        [MaxLength(20)]
        public string City { get; set; }
        public int? Sex { get; set; }
    }
}
