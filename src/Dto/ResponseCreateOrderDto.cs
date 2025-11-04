using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto de respuesta para la operacion de creacion de orden.
    /// Contiene toda la informacion de la orden creada.
    /// </summary>
    public class ResponseCreateOrderDto
    {
        /// <summary>
        /// Identificador unico (UUID V4) de orden.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nombre de cliente (Usuario) que realizo la orden.
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
        /// Estado de la orden (Pendiente al recien crearse).
        /// </summary>
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Fecha de creacion de orden.
        /// </summary>
        public DateOnly CreateAt { get; set; }

        /// <summary>
        /// Precio total de orden.
        /// </summary>
        public double TotalPrice { get; set; }

        /// <summary>
        /// Listado de productos de orden.
        /// </summary>
        public List<OrderItem> Items { get; set; } = new();

    }
}