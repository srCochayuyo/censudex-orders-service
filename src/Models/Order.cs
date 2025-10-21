using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public int OrderNumber { get; set; }

        public string Address { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }

        [RegularExpression("Pendiente|En Procesamiento|Enviado|Entregado|Cancelado")]
        public string OrderStatus { get; set; } = string.Empty!;

        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public decimal TotalPrice => Items.Sum(i => i.Subtotal);
        
    }
    
}