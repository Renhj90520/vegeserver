using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        [MaxLength(32)]
        public string Code { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        [MaxLength(70)]
        public string QR_Path { get; set; }
        /// <summary>
        /// 0:停用，1:启用
        /// </summary>
        public int State { get; set; } = 1;
    }
}
