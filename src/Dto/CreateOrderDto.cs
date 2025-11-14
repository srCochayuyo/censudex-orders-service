using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto para la creacion de una nueva orden.
    /// Contiene los datos necesarios para crear la orden en el sistema.
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// Identificador unico (UUID V4) de usuario. 
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre de Cliente (Usuario).
        /// </summary>
        public string UserName { get; set; } = string.Empty!;

        /// <summary>
        /// correo electronico Cliente (Usuario).
        /// </summary>
        public string UserEmail { get; set; } = string.Empty!;

        /// <summary>
        /// Direccion de envio de Order.
        /// </summary>
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// Lista de productos pertenecientes a la Order.
        /// </summary>
        public List<CreateOrderItemDto> Items { get; set; } = new();
    }
}