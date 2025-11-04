using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta para la operacion de visualizacion historica de ordenes.
    /// Contiene toda la informacion de una orden.
    /// </summary>
    public class ResponseGetOrderAdminDto
    {
        /// <summary>
        /// Identificador unico (UUID V4) de orden.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador unico (UUID V4) de cliente (Usuario).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre de cliente (Usuario).
        /// </summary>
        public string UserName { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de orden.
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty!;

        /// <summary>
        /// Direccion de envio de orden.
        /// </summary>
        public string Address { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de seguimiento de envio de orden.
        /// </summary>
        public string? TrackingNumber { get; set; }
        
        /// <summary>
        /// Estado de orden.
        /// </summary>
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
        /// Listado de productos de orden.
        /// </summary>
        public List<OrderItem> Items { get; set; } = new();

        /// <summary>
        /// Precio total de orden.
        /// </summary>
        public double TotalPrice { get; set; }
    }
}