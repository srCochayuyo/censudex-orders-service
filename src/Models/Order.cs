using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        public string UserName { get; set; } = string.Empty!;

        public string OrderNumber { get; set; } = string.Empty!;

        public string Address { get; set; } = string.Empty!;

        public string? TrackingNumber { get; set; }

        [RegularExpression("Pendiente|En Procesamiento|Enviado|Entregado|Cancelado")]
        public string OrderStatus { get; set; } = string.Empty!;

        public DateOnly CreateAt { get; set; }

        public DateOnly? UpdateAt { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public double TotalPrice => Items.Sum(i => i.Subtotal);
        
    }
    
}