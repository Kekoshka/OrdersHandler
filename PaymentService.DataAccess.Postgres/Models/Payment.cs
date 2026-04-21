using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.DataAccess.Postgres.Models
{
    public class Payment
    {
        /// <summary>
        /// Id платежа
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// Id заказа
        /// </summary>
        public long OrderId { get; set; }
        
        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// Дата создания платежа
        /// </summary>
        public DateTime DateCreate {get;set;}
        public bool Status { get; set; }
    }
}
