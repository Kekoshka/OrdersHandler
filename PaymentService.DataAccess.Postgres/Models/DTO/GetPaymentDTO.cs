using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Models.DTO
{
    public class GetPaymentDTO
    {
        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Статус платежа, true - оплачено false - не оплачено
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Дата создания платежа
        /// </summary>
        public DateTime DateCreate { get; set; }
    }
}
