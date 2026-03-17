using MediatR;
using PaymentService.DataAccess.Postgres.Models.DTO;

namespace PaymentService.WebApi.Mediatr.Commands
{
    public class CreatePaymentCommand : IRequest<long>
    {
        /// <summary>
        /// Id заказа
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Сумма платежа
        /// </summary>
        public decimal Price { get; set; }
    }
}
