using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Models
{
    /// </summary>
    /// Representa un item o producto dentro de la orden.
    /// Contiene la informacion del producto.
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// Identificador unico del item de la orden.
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador unico de order a la que pertenece el item.
        /// </summary>
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }

        /// <summary>
        /// Identificador unico del producto asociado a este item.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Nombre del producto asociado al item.
        /// </summary>
        public string ProductName { get; set; } = string.Empty!;

        /// <summary>
        /// Cantidad de unidades del producto incluidas en la ordenr.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Subtotal correspondiente a este item.
        /// Se calcula multiplicando la cantidad por el precio unitario.
        /// </summary>
        public double Subtotal => Quantity * UnitPrice;

    }
}