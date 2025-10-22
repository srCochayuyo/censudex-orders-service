using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.src.Dto;
using OrderService.src.Interfaces;

namespace OrderService.src.Controller
{
    [Route("Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderDto request)
        {
            try
            {
            
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var result = await _orderRepository.CreateOrder(request);

                return Ok(new
                {
                    message = "Pedido creado con exito",
                    Estacion = result
                });
            }
            catch (Exception e)
            {
                
                return StatusCode(500, new { message = e.Message });
            }
        }



    }
}