using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta para la operacion de cambio de estado de orden.
    /// Contiene la informacion de envio de orden.
    /// </summary>
    public class ResponseOrderStateDto
    {
        /// <summary>
        /// Numero de orden.
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty!;

        /// <summary>
        /// Estado de la orden.
        /// </summary>
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Fecha de ultima actualizacion de orden.
        /// </summary>
        public DateOnly? UpdateAt { get; set; }
    }
}