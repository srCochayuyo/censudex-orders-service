using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class ResponseGetOrderUserDto
    {

        public string OrderNumber { get; set; } = string.Empty!;

        public string Address { get; set; } = string.Empty!;

        public string OrderStatus { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }

        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

        public decimal TotalPrice { get; set; }

        public List<ItemsOrderUserDto> Items { get; set; } = new();
    }
}