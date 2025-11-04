using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta utilizado para mostrar al cliente 
    /// Contiene toda informacion revelante de una orden.
    /// </summary>
    public class ItemsOrderUserDto
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty!;

        /// <summary>
        /// Precio unitario de producto.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Cantidad solictada de producto.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Subtotal de producto solicitado
        /// </summary>
        public double Subtotal { get; set; }
    }
}