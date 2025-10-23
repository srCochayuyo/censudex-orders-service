using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Dto;
using OrderService.src.Models;

namespace OrderService.src.Mappers
{
    public static class OrderMappers
    {
        public static ResponseCreateOrderDto ToCreateOrderResponse(this Order order)
        {
            return new ResponseCreateOrderDto
            {
                Id = order.Id,
                UserName = order.UserName,
                OrderNumber = order.OrderNumber,
                Address = order.Address,
                OrderStatus = order.OrderStatus,
                TrackingNumber = order.TrackingNumber,
                CreateAt = order.CreateAt,
                TotalPrice = order.TotalPrice,
                Items = order.Items
            };
        }

        public static ResponseChangeStateDto ToChangeStateResponse(this Order order)
        {
            return new ResponseChangeStateDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                Address = order.Address,
                OrderStatus = order.OrderStatus,
                TrackingNumber = order.TrackingNumber,
                CreateAt = order.CreateAt,
                UpdateAt = order.UpdateAt
            };
        }

        public static ResponseOrderStateDto ToOrderStateResponse(this Order order)
        {
            return new ResponseOrderStateDto
            {
                OrderNumber = order.OrderNumber,
                OrderStatus = order.OrderStatus,
                UpdateAt = order.UpdateAt

            };
        }

        public static ResponseGetOrderUserDto ToGetOrderUserResponse(this Order order)
        {
            return new ResponseGetOrderUserDto
            {
                OderId = order.Id,
                OrderNumber = order.OrderNumber,
                Address = order.Address,
                OrderStatus = order.OrderStatus,
                TrackingNumber = order.TrackingNumber,
                CreateAt = order.CreateAt,
                UpdateAt = order.UpdateAt,
                TotalPrice = order.TotalPrice,
                Items = order.Items.Select(i => new ItemsOrderUserDto
                {
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
        
        public static ResponseGetOrderAdminDto ToGetOrdersResponse(this Order order)
        {
            return new ResponseGetOrderAdminDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.UserName,
                OrderNumber = order.OrderNumber,
                Address = order.Address,
                TrackingNumber = order.TrackingNumber,
                OrderStatus = order.OrderStatus,
                CreateAt = order.CreateAt,
                UpdateAt = order.UpdateAt,
                Items = order.Items
            };
        }
    }
}