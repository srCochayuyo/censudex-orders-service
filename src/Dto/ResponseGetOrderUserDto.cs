using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta para la visualizacion de ordenes de un cliente (Usuario).
    /// Contiene todos los datos de interes para un cliente de sus ordenes.
    /// </summary>
    public class ResponseGetOrderUserDto
    {

        /// <summary>
        /// Identificador unico (UUID V4) de orden. 
        /// </summary>
        public Guid OderId { get; set; }
        
        /// <summary>
        /// Numero de orden.
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty!;

        /// <summary>
        /// Direccio de envio de orden.
        /// </summary>
        public string Address { get; set; } = string.Empty!;

        /// <summary>
        /// Estado de orden.
        /// </summary>
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de seguimiento de envio de orden.
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Fecha de creacion de orden.
        /// </summary>
        public DateOnly CreateAt { get; set; }

        /// <summary>
        /// Fecha de ultima actualizacion de orden.
        /// </summary>
        public DateOnly? UpdateAt { get; set; }

        /// <summary>
        /// Precio total de orden.
        /// </summary>
        public double TotalPrice { get; set; }

        /// <summary>
        /// Listado de productos de orden.
        /// </summary>
        public List<ItemsOrderUserDto> Items { get; set; } = new();
    }
}