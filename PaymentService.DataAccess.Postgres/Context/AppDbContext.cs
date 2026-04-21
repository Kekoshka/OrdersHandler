using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Context
{
    /// <summary>
    /// Контекст для общения с базой данных платежей
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Модель таблицы платежей
        /// </summary>
        public DbSet<Payment> Payments { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
