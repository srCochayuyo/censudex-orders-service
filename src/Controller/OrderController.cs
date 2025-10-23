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

        [HttpGet("{Identifier}")]
        public async Task<IActionResult> GetOrderByIdentifier(string Identifier)
        {
            try
            {

                Guid? orderId = null;
                string? orderNumber = null;

                if (Guid.TryParse(Identifier, out var guid))
                {
                    orderId = guid;
                }
                else
                {
                    orderNumber = Identifier;
                }

                var order = await _orderRepository.GetOrderByIdentifier(orderId, orderNumber);

                if (order == null)
                {
                    return NotFound("Error: No se econtro el Pedido");
                }

                var result = new
                {
                    message = "Pedido obtenido con exito",
                    Order = order
                };

                return Ok(result);

            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPut("ChangeState/{Identifier}")]
        public async Task<IActionResult> ChangeState(string Identifier, ChangeStateDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                Guid? orderId = null;
                string? orderNumber = null;

                if (Guid.TryParse(Identifier, out var guid))
                {
                    orderId = guid;
                }
                else
                {
                    orderNumber = Identifier;
                }

                var NewState = await _orderRepository.ChangeStateOrder(orderId, orderNumber, request);

                return Ok(new
                {
                    message = "Estado de pedido modificado con exito",
                    Estacion = NewState
                });

            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpPut("Cancelate/{Identifier}")]
        public async Task<IActionResult> CancelateOrder(string Identifier)
        {
            try
            {

                Guid? orderId = null;
                string? orderNumber = null;

                if (Guid.TryParse(Identifier, out var guid))
                {
                    orderId = guid;
                }
                else
                {
                    orderNumber = Identifier;
                }

                var cancelate = await _orderRepository.CancelateOrder(orderId, orderNumber);

                return Ok(new
                {
                    message = "Pedido Canccelado con exito",
                    Estacion = cancelate
                });

            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet("Orders")]
        public async Task<IActionResult> GetOrdersUser(Guid UserId)
        {
            var Orders = await _orderRepository.GetAllOrdersUser(UserId);

            var response = new
            {
                message = "Lista de pedidos obtenida exitosamente",
                Estaciones = Orders
            };

            return Ok(response);
        }



    }
}