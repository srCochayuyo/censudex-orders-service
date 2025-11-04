using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using OrderService.Grpc;
using OrderService.src.Dto;
using OrderService.src.Models;

namespace OrderService.src.Mappers
{
    /// <summary>
    /// Clase para mapear los DTOs de respuesta a mensajes protobuf.
    /// Proporciona métodos estáticos para la conversión de objetos a respuestas gRPC.
    /// </summary>
    public static class ProtoMappers
    {
        /// <summary>
        /// Convierte un DTO de creacion de order a una respuesta protobuf.
        /// Metodo que mapea todos los campos del DTO al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// DTO con los datos de order creada.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con los datos de order.
        /// </returns
        public static CreateOrderResponse ToCreateOrderProtoResponse(ResponseCreateOrderDto request)
        {
            return new CreateOrderResponse
            {
                Id = request.Id.ToString(),
                UserName = request.UserName,
                OrderNumber = request.OrderNumber,
                Address = request.Address,
                OrderStatus = request.OrderStatus,
                CreateAt = request.CreateAt.ToString("yyyy-MM-dd"),
                TotalPrice = request.TotalPrice,
                Items =
                {
                    request.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id.ToString(),
                        OrderId = i.OrderId.ToString(),
                        ProductId = i.ProductId.ToString(),
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Subtotal = i.Subtotal
                    })
                }
            };
        }

        /// <summary>
        /// Convierte una lista de DTOs orders a una respuesta protobuf.
        /// Metodo que mapea todos los campos de los DTOs al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// Lista de DTOs con el estado de orders.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con los estados de orders.
        /// </returns>
        public static GetOrderStatusResponse ToGetOrderStatusProtoResponse(List<ResponseOrderStateDto> request)
        {
            var response = new GetOrderStatusResponse();
            response.Orders.AddRange(request.Select(o => new OrderStateInfo
            {
                OrderNumber = o.OrderNumber,
                OrderStatus = o.OrderStatus,
                UpdateAt = o.UpdateAt.ToString()
            }));
            return response;
        }

        /// <summary>
        /// Convierte un DTO de cambio de estado de order a una respuesta protobuf.
        /// Metodo que mapea todos los campos del DTO al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// DTO con los datos de order despues del cambio de estado.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con los datos actualizados de order.
        /// </returns>

        public static ChangeOrderStateResponse ToChangeOrderStateProtoResponse(ResponseChangeStateDto request)
        {
            return new ChangeOrderStateResponse
            {
                Id = request.Id.ToString(),
                OrderNumber = request.OrderNumber,
                Address = request.Address,
                OrderStatus = request.OrderStatus,
                TrackingNumber = request.TrackingNumber ?? "",
                CreateAt = request.CreateAt.ToString("yyyy-MM-dd"),
                UpdateAt = request.UpdateAt?.ToString("yyyy-MM-dd")
            };
        }

        /// <summary>
        /// Convierte un DTO de cancelacion de order a una respuesta protobuf.
        /// Metodo que mapea todos los campos del DTO al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// DTO con los datos de order cancelada.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con la informacion de la order cancelada.
        /// </returns>
        public static CancelOrderResponse ToCancelOrderProtoResponse(ResponseChangeStateDto request)
        {
            return new CancelOrderResponse
            {
                Id = request.Id.ToString(),
                OrderNumber = request.OrderNumber,
                Address = request.Address,
                OrderStatus = request.OrderStatus,
                TrackingNumber = request.TrackingNumber ?? "",
                CreateAt = request.CreateAt.ToString("yyyy-MM-dd"),
                UpdateAt = request.UpdateAt?.ToString("yyyy-MM-dd")

            };
        }

        /// <summary>
        /// Convierte una lista de DTOs de orders de usuario a una respuesta protobuf.
        /// Metodo que mapea todos los campos de los DTOs al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// Lista de DTOs con las orders del usuario.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con orders del usuario.
        /// </returns>
        public static GetUserOrdersResponse ToGetUserOrdersProtoResponse(List<ResponseGetOrderUserDto> request)
        {
            var response = new GetUserOrdersResponse();
            response.Orders.AddRange(request.Select(o => new UserOrderInfo
            {
                OrderId = o.OderId.ToString(),
                OrderNumber = o.OrderNumber,
                Address = o.Address,
                OrderStatus = o.OrderStatus,
                TrackingNumber = o.TrackingNumber ?? "",
                CreateAt = o.CreateAt.ToString(),
                UpdateAt = o.UpdateAt.ToString(),
                TotalPrice = o.TotalPrice,
                Items = { o.Items.Select(i => new UserOrderItemInfo
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                })}
            }));
            return response;
        }
    
        /// <summary>
        /// Convierte una lista historica de DTOs de ordes a una respuesta protobuf.
        /// Metodo que mapea todos los campos de los DTOs al mensaje gRPC correspondiente.
        /// </summary>
        /// <param name="request">
        /// Lista de DTOs con orders.
        /// </param>
        /// <returns>
        /// Respuesta protobuf con orders.
        /// </returns>
        public static GetAdminOrdersResponse ToGetAdminOrdersProtoResponse(List<ResponseGetOrderAdminDto> request)
        {
            var response = new GetAdminOrdersResponse();
            response.Orders.AddRange(request.Select(o => new AdminOrderInfo
            {
                Id = o.Id.ToString(),
                UserId = o.UserId.ToString(),
                UserName = o.UserName,
                OrderNumber = o.OrderNumber,
                Address = o.Address,
                TrackingNumber = o.TrackingNumber ?? "",
                OrderStatus = o.OrderStatus,
                CreateAt = o.CreateAt.ToString(),
                UpdateAt = o.UpdateAt.ToString(),
                TotalPrice = o.TotalPrice,
                Items = { o.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id.ToString(),
                    OrderId = i.OrderId.ToString(),
                    ProductId = i.ProductId.ToString(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                })}
            }));
            return response;
        }
    }
}