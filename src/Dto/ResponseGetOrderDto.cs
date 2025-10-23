using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Dto
{
    public class ResponseGetOrderDto
    {
        public Guid Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty!;

        public string Address { get; set; } = string.Empty!;

        public string OrderStatus { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }

        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

        public decimal TotalPrice { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        
    }
}