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
    public static class ProtoMappers
    {
        public static CreateOrderResponse ToCreateOrderProtoResponse(ResponseCreateOrderDto request)
        {
            return new CreateOrderResponse
            {
                Id = request.Id.ToString(),
                UserName = request.UserName,
                OrderNumber = request.OrderNumber,
                Address = request.Address,
                OrderStatus = request.OrderStatus,
                TrackingNumber = request.TrackingNumber ?? string.Empty,
                CreateAt = request.CreateAt.ToString("yyyy-MM-dd"),
                TotalPrice = (double)request.TotalPrice,
                Items =
                {
                    request.Items.Select(i => new OrderItemResponse
                    {
                        Id = i.Id.ToString(),
                        OrderId = i.OrderId.ToString(),
                        ProductId = i.ProductId.ToString(),
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = (double)i.UnitPrice,
                        Subtotal = (double)i.Subtotal
                    })
                }
            };
        }

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
                TotalPrice = (double)o.TotalPrice,
                Items = { o.Items.Select(i => new UserOrderItemInfo
                {
                    ProductName = i.ProductName,
                    UnitPrice = (double)i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = (double)i.Subtotal
                })}
            }));
            return response;
        }

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
                TotalPrice = (double)o.TotalPrice,
                Items = { o.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id.ToString(),
                    OrderId = i.OrderId.ToString(),
                    ProductId = i.ProductId.ToString(),
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = (double)i.UnitPrice,
                    Subtotal = (double)i.Subtotal
                })}
            }));
            return response;
        }
    }
}