using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.WebApi.Common.DTO
{
    /// <summary>
    /// DTO обновления статуса платежа
    /// </summary>
    public class UpdatePaymentDTO
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Статус платежа
        /// </summary>
        public bool Status { get; set; }
    }
}
