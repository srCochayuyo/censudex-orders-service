using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService.src.Models;

namespace OrderService.src.Helper
{
    public class OrderHelpers
    {

        public static (Guid? Id, string? OrderNumber) ParseOrderIdentifier(string? orderIdentifier)
        {
            Guid? Id = null;
            string? OrderNumber = null;

            if (Guid.TryParse(orderIdentifier, out var guid))
            {
                Id = guid;

            }
            else
            {
                OrderNumber = orderIdentifier;
            }

            return (Id, OrderNumber);

        }



        public static IQueryable<Order> AdminFilter(IQueryable<Order> query,Guid? UserId, string? UserName, Guid? OrderId, string? OrderNumber, DateOnly? InitialDate, DateOnly? FinishDate)
        {
            if (UserId != null)
            {
                query = query.Where(o => o.UserId == UserId);
            }

            if (UserName != null)
            {
                query = query.Where(o => o.UserName.ToLower().Trim() == UserName.ToLower().Trim());
            }

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