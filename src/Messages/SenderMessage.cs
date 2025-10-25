using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Messages
{
    public class SenderMessage
    {
        public Guid OrderId { get; set; }

        public string SendBy { get; set; } = "Order-Service";

        public DateOnly SendAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public List<OrderItemsMessage> Items { get; set; } = new();

    

        public class OrderItemsMessage
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}