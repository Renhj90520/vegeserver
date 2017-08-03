using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class WXConfig
    {
        public bool debug { get; set; } = false;
        public string appId { get; set; }
        public string timestamp { get; set; }
        public string nonceStr { get; set; }
        public string signature { get; set; }
        public List<string> jsApiList { get; set; } = new List<string>() { "chooseWXPay", "getLocation" };
        public string prepayid { get; set; }
        public string key { get; set; }

    }
}
