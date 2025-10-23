using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class ItemsOrderUserDto
    {
        public string ProductName { get; set; } = string.Empty!;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }
    }
}