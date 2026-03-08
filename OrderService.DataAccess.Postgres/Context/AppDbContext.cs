using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DataAccess.Postgres.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public AppDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=OrderService;User Id=postgres;Password=31121985;");
        }
    }
}
