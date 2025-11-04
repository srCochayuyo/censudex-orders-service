using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    /// <summary>
    /// Dto para el cambio de estado de alguna Order (Pedido)
    /// Contiene los datos y validaciones necesarias para realizar la operacione
    /// </summary>
    public class ChangeStateDto
    {

        /// <summary>
        /// Nuevo estado de orden (Obligatorio)
        /// </summary>
        [Required(ErrorMessage = "El Estado es requerido")]
        [RegularExpression(@"^(?i)(pendiente|en\s?procesamiento|enviado|entregado)$", ErrorMessage = "Estado inválido. Valores permitidos: Pendiente, En Procesamiento, Enviado, Entregado")]
        public string OrderStatus { get; set; } = string.Empty!;

        /// <summary>
        /// Numero de seguimiento de envio de la Orden para rastreo del mismo (solo cuando el nuevo estado es "Enviado")
        /// </summary>
        public string? TrackingNumber { get; set; }
        
    }
}