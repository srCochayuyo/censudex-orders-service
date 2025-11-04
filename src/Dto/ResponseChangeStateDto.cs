using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta para la operacion de cambio de estado de pedido.
    /// Contiene toda la informacion de gestion de envio asociada a una orden.
    /// </summary>
    public class ResponseChangeStateDto
    {
        /// <summary>
        /// Identificador unico (UUID V4) de Orden.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Numero de Orden.
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty!;

        /// <summary>
        /// Direccion de envio de Orden.
        /// </summary>
        public string Address { get; set; } = string.Empty!;

        /// <summary>
        ///  Estado de orden.
        /// </summary>
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de seguimiento de envio de orden (Nulo si aun no se envia).
        /// </summary>
        public string? TrackingNumber { get; set; }

        /// <summary>
        /// Fecha de creacion de orden.
        /// </summary>   
        public DateOnly CreateAt { get; set; }

        /// <summary>
        /// Fecha de ultima actualizacion de orden (Nulo si aun no hay modificacion en la order desde la creacion).
        /// </summary>
        public DateOnly? UpdateAt { get; set; }

    }
}