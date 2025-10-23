using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class ResponseOrderStateDto
    {
        public string OrderNumber { get; set; } = string.Empty!;

        public string OrderStatus { get; set; } = string.Empty!;

        public DateOnly? UpdateAt { get; set; }
    }
}