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
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

    }
}
