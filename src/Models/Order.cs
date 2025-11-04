using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Models
{
    /// <summary>
    /// Clase que representa la entidad Order en el sistema.
    /// Contiene la informacion principal de Order.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Identificador unico orden.
        /// Se genera automáticamente al crear una nueva instancia.
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador unico de usuario.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string UserName { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de orden.
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty!;

        /// <summary>
        /// Direccion de envio.
        /// </summary>
        public string Address { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de seguimiento de envio de la Orden para rastreo de la misma.
        /// <summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Estado actual de orden.
        /// Los valores validos son: "Pendiente", "En Procesamiento", "Enviado", "Entregado" o "Cancelado".
        /// </summary>
        [RegularExpression("Pendiente|En Procesamiento|Enviado|Entregado|Cancelado")]
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Fecha de creacion de orden.
        /// </summary>
        public DateOnly CreateAt { get; set; }

        /// <summary>
        /// Fecha de ultima actualizacion de orden.
        /// </summary>
        public DateOnly? UpdateAt { get; set; }

        /// <summary>
        /// Listado de productos asociados a orden.
        /// </summary>
        public List<OrderItem> Items { get; set; } = new();

        /// <summary>
        /// Precio total de orden
        /// Se calcula automáticamente sumando el subtotal de todos los productos.
        public double TotalPrice => Items.Sum(i => i.Subtotal);
        
    }
    
}