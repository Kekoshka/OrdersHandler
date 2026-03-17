using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Models
{
    public class Status
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
