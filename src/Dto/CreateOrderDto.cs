using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty!;
        
        public string Address { get; set; } = string.Empty;
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}