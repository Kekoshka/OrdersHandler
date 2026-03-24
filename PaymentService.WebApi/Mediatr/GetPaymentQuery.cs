using MediatR;
using PaymentService.WebApi.Common.DTO;
using PaymentService.WebApi.Interfaces;

namespace PaymentService.WebApi.Mediatr
{
    /// <summary>
    /// Запрос на получение платежа
    /// </summary>
    public class GetPaymentQuery : IRequest<GetPaymentDTO>
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long PaymentId { get; set; }
    }

    /// <summary>
    /// Обработчик запроса на получение платежа
    /// </summary>
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, GetPaymentDTO>
    {
        /// <summary>
        /// Сервис платежей
        /// </summary>
        IPaymentService _paymentService;
        public GetPaymentQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Получение DTO платежа по коду платежа
        /// </summary>
        /// <param name="request">Запрос на получение платежа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>DTO платежа</returns>
        public async Task<GetPaymentDTO> Handle(GetPaymentQuery request, CancellationToken cancellationToken) =>
            await _paymentService.GetPaymentAsync(request.PaymentId, cancellationToken);
    }
}
