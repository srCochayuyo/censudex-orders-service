using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.src.Dto;
using OrderService.src.Helper;
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
                    Order= result
                });
            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet("State/{Identifier}")]
        public async Task<IActionResult> GetOrderStatusByIdentifier(string Identifier)
        {
            try
            {

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(Identifier);

                var order = await _orderRepository.GetOrderStateByIdentifier(OrderId, OrderNumber);

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


                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(Identifier);
               
                var NewState = await _orderRepository.ChangeStateOrder(OrderId, OrderNumber, request);

                return Ok(new
                {
                    message = "Estado de pedido modificado con exito",
                    Order = NewState
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

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(Identifier);

                var cancelate = await _orderRepository.CancelateOrder(OrderId, OrderNumber);

                return Ok(new
                {
                    message = "Pedido Canccelado con exito",
                    Order = cancelate
                });

            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        [HttpGet("Orders/{UserId}")]
        public async Task<IActionResult> GetOrdersUser(Guid UserId, [FromQuery] string? OrderIdentifier, [FromQuery] DateOnly? InitialDate, [FromQuery] DateOnly? FinishDate)
        {

            try
            {

                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(OrderIdentifier);

                var orders = await _orderRepository.GetAllOrdersUser(UserId, OrderId, OrderNumber, InitialDate, FinishDate);

                var response = new
                {
                    message = "Lista de pedidos obtenida exitosamente",
                    Orders = orders
                };

                return Ok(response);




            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }



        }
        
        [HttpGet("Orders")]
        public async Task<IActionResult> GetOrdersAdmin([FromQuery]string? UserIdentifier, [FromQuery] string? OrderIdentifier, [FromQuery] DateOnly? InitialDate, [FromQuery] DateOnly? FinishDate)
        {

            try
            {
                Guid? UserId = null;
                string? UserName = null;

                if (Guid.TryParse(UserIdentifier, out var guid))
                {
                    UserId = guid;

                }
                else
                {
                    UserName = UserIdentifier;
                }


                var (OrderId, OrderNumber) = OrderHelpers.ParseOrderIdentifier(OrderIdentifier);

                var orders = await _orderRepository.GetAllOrdersAdmin(UserId,UserName, OrderId, OrderNumber,InitialDate,FinishDate);

                var response = new
                {
                    message = "Lista de pedidos obtenida exitosamente",
                    Orders = orders
                };

                return Ok(response);

            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }



        }
        



    }
}