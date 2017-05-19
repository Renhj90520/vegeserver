﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Vege.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int OpenId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime CancelTime { get; set; }
        public DateTime FinishTime { get; set; }
        /// <summary>
        /// 0:Cancel,1:未联系,2:派送中,3:交易完成
        /// </summary>
        public int State { get; set; }
        public ICollection<OrderItem> Products { get; set; }

    }
}