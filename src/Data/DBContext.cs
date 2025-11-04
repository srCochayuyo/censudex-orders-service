using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.src.Models;

namespace OrderService.src.Data
{
    /// <sumary>
    /// Contexto de la base de datos del servicio.
    /// Proporciona acceso a las entidades y maneja las operaciones de base de datos
    /// <sumary>

    public class DBContext : DbContext
    {
        /// <summary>
        /// Constructor de la clase contexto de la base de datos
        /// </summary>
        /// <param name="options">Opciones de configuracion para el contexto</param>
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        /// <summary>
        /// Entidad Order (Orden/Pedido) Para operaciones CRUD en la tabla Orders
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// Entidad OrderItems la cual representa los items que se encuentran en una order.
        /// </summary>
        /// <value></value>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

    }
}