using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class ResponseChangeStateDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string OrderNumber { get; set; } = string.Empty!;

        public string Address { get; set; } = string.Empty!;

        public string OrderStatus { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }
        
        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

    }
}