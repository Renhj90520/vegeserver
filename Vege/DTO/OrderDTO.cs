using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public string OpenId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime CancelTime { get; set; }
        public DateTime FinishTime { get; set; }
        /// <summary>
        /// 0:未联系,1:派送中,2:Cancel,3:交易完成
        /// </summary>
        public int State { get; set; }
        public IEnumerable<OrderItemDTO> Products { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Name { get; set; }
        public string Area { get; set; }
    }
}
