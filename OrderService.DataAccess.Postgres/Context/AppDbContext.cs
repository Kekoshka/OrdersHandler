using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DataAccess.Postgres.Context
{
    public class AppDbContext : DbContext
    {
        AppDbContext()
        {
            Database.EnsureCreated();
        }
    }
}
