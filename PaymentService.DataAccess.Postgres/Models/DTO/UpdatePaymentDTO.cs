using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Models.DTO
{
    public class UpdatePaymentDTO
    {
        public long Id { get; set; }
        public long StatusId { get; set; }
    }
}
