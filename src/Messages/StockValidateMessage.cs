using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Messages
{
    public class StockValidateMessage
    {
        public string EventType { get; set; } = string.Empty!;
        public Guid ProductId { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public Guid OrderId { get; set; }
        public string Timestamp { get; set; } = string.Empty!;
        public bool ValidationResult { get; set; }
    }
}