using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Helper
{
    public class OrderHelpers
    {

        public static (Guid? OrderId, string? OrderNumber) ParseOrderIdentifier(string? orderIdentifier)
        {
            Guid? OrderId = null;
            string? OrderNumber = null;

            if (Guid.TryParse(orderIdentifier, out var guid))
            {
                OrderId = guid;

            }
            else
            {
                OrderNumber = orderIdentifier;
            }

            return (OrderId, OrderNumber);

        }



        public static IQueryable<Order> UserFilter(IQueryable<Order> query, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate)
        {
            if (OrderId != null)
            {
                query = query.Where(o => o.Id == OrderId);
            }

            if (!string.IsNullOrWhiteSpace(OrderNumber))
            {
                query = query.Where(o => o.OrderNumber == OrderNumber);
            }

            if (InitialDate != null)
            {
                query = query.Where(o => o.CreateAt >= InitialDate);
            }

            if (FinishDate != null)
            {
                query = query.Where(o => o.CreateAt <= FinishDate);
            }

            return query;

        }  
    }
}