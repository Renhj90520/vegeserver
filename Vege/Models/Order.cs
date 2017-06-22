using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Order
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(28)]
        public string OpenId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime CancelTime { get; set; }
        public DateTime FinishTime { get; set; }
        /// <summary>
        /// 0:未联系,1:派送中,2:Cancel,3:交易完成,4:删除
        /// </summary>
        public int State { get; set; }
        public ICollection<OrderItem> Products { get; set; }
        public int AddressId { get; set; }
        [MaxLength(160)]
        public string CancelReason { get; set; }
        public double DeliveryCharge { get; set; } = 0;
    }
}
