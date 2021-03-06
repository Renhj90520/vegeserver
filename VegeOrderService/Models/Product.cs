﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VegeOrderService.Models
{
    public class Product
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
        public double TotalCount { get; set; }
        public int UnitId { get; set; }
        [MaxLength(10)]
        public string UnitName { get; set; }
        public double Step { get; set; }
        public double Price { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int State { get; set; } = 1;
        public ICollection<Picture> Pictures { get; set; }
        public int CategoryId { get; set; }
    }
}
