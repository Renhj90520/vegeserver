using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VegeOrderService.Models
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
        /// 0:未支付,1:已支付,2.已联系,3:派送中,4:Cancel,5:交易完成,6:已退款,7:删除
        /// </summary>
        public int State { get; set; }
        public ICollection<OrderItem> Products { get; set; }
        public int AddressId { get; set; }
        [MaxLength(160)]
        public string CancelReason { get; set; }
        public double DeliveryCharge { get; set; } = 0;
        /// <summary>
        /// 0:新订单未通知,1:新订单已通知,2:已支付未通知,3:已支付已通知
        /// </summary>
        [MaxLength(1)]
        public string NotifyState { get; set; } = "0";
        [MaxLength(32)]
        public string WXOrderId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [MaxLength(160)]
        public string RefundNote { get; set; }
        [MaxLength(1)]
        public string IsPaid { get; set; } = "0";
    }
}
