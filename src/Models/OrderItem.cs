using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Models
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal => Quantity * UnitPrice;

    }
}