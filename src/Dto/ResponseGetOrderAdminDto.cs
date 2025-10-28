using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Dto
{
    public class ResponseGetOrderAdminDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty!;

        public string OrderNumber { get; set; } = string.Empty!;

        public string Address { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }
        
        public string OrderStatus { get; set; } = string.Empty!;

        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public double TotalPrice { get; set; }
    }
}