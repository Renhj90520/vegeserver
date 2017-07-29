using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.DTO
{
    public class PwdWrapper
    {
        public string UserName { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
    }
}
