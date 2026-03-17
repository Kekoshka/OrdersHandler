using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Status> Statuses { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
