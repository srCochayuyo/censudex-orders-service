using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.src.Dto
{
    public class ChangeStateDto
    {


        [Required(ErrorMessage = "El Estado es requerido")]
        [RegularExpression(@"^(?i)(pendiente|en\s?procesamiento|enviado|entregado)$", ErrorMessage = "Estado inválido. Valores permitidos: Pendiente, En Procesamiento, Enviado, Entregado")]
        public string OrderStatus { get; set; } = string.Empty!;

        public string? TrackingNumber{ get; set; }
        
        
    }
}