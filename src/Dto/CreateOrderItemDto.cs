using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto para crear los productos dentro de la Orden.
    /// Contiene los datos necesarios para la identificacion de los productos almacenados en microservicio
    /// correspondiente, ademas de datos de interes para el procesamiento de la orden.
    /// </summary>
    public class CreateOrderItemDto
    {
        /// <summary>
        /// Identificador unico (UUID V4) de producto.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        ///  Nombre de producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty!;

        /// <summary>
        /// Cantidad deseada de producto.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario de producto.
        /// </summary>
        public double UnitPrice { get; set; }
    }
}