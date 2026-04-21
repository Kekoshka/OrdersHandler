using MediatR;
using PaymentService.WebApi.Common.Mappers;
using PaymentService.WebApi.Interfaces;

namespace PaymentService.WebApi.Mediatr
{
    /// <summary>
    /// Команда обновления платежа 
    /// </summary>
    public class UpdatePaymentCommand : IRequest
    {
        /// <summary>
        /// Код платежа
        /// </summary>
        public long PaymentId { get; set; }
        /// <summary>
        /// Статус платежа
        /// </summary>
        public bool Status { get; set; }
    }

    /// <summary>
    /// Обработчик команды обновления платежа
    /// </summary>
    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand>
    {
        /// <summary>
        /// Сервис платежей
        /// </summary>
        IPaymentService _paymentService;
        public UpdatePaymentCommandHandler(IPaymentService paymentService,
        PaymentMapper paymentMapper)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Обновляет статус платежа
        /// </summary>
        /// <remarks>
        /// 0 - не оплачен
        /// 1 - оплачен
        /// </remarks>
        /// <param name="request">Команда обновления статуса платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        public async Task Handle(UpdatePaymentCommand request, CancellationToken cancellationToken) =>
            await _paymentService.UpdatePaymentAsync(request.PaymentId, request.Status, cancellationToken);
    }
}
