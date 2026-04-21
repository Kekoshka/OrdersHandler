using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.DataAccess.Postgres.Models
{
    /// <summary>
    /// Доменная модель заказа
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long ProductId { get; set; }
        public int Amount { get; set; }
        public string EmailClient { get; set; }
        public decimal Price { get; set; }
        public string PhoneNumber { get; set; }
    }
}
