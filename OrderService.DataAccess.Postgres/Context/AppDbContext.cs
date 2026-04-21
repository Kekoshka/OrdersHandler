using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DataAccess.Postgres.Context
{
    /// <summary>
    /// Контекст для общения с базой данных заказов
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Модель таблицы заказов
        /// </summary>
        public DbSet<Order> Orders { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    }
}
