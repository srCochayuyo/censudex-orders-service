using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Messages
{
    /// <summary>
    /// Clase que representa el mensaje enviado entre servicios para comunicar informacion de una orden atraves de RabbitMQ.
    /// Contiene los datos principales de la orden y los productos asociados.
    /// </summary>
    public class SenderMessage
    {
        /// <summary>
        /// Identificador unico de orden asociada al mensaje.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Nombre del servicio que envia el mensaje.
        /// </summary>
        public string SendBy { get; set; } = "Order-Service";

        /// <summary>
        /// Fecha de envio del mensaje.
        /// </summary>
        public DateOnly SendAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        /// <summary>
        /// Listado de productos incluidos en la orden.
        /// </summary>
        public List<OrderItemsMessage> Items { get; set; } = new();

        /// <summary>
        /// Clase interna que representa los datos de cada producto dentro del mensaje.
        /// Incluye el identificador unico del producto y la cantidad solicitada.
        /// </summary>
        public class OrderItemsMessage
        {
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}