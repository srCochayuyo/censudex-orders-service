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
        public static ResponseOrderDto ToOrderResponse(this Order order)
        {
            return new ResponseOrderDto
            {
                Id = order.Id,
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
    }
}