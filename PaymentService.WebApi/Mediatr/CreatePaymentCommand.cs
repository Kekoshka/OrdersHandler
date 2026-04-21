using MediatR;
using PaymentService.WebApi.Common.Mappers;
using PaymentService.WebApi.Interfaces;

namespace PaymentService.WebApi.Mediatr
{
    /// <summary>
    /// Команда создания платежа
    /// </summary>
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

    /// <summary>
    /// Обработчик команды создания платежа
    /// </summary>
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, long>
    {
        /// <summary>
        /// Сервис платежей
        /// </summary>
        IPaymentService _paymentService;
        
        /// <summary>
        /// Маппер платежей
        /// </summary>
        PaymentMapper _paymentMapper;
        public CreatePaymentCommandHandler(IPaymentService paymentService,
            PaymentMapper paymentMapper)
        {
            _paymentService = paymentService;
            _paymentMapper = paymentMapper;
        }
        
        /// <summary>
        /// Создание платежа
        /// </summary>
        /// <param name="request">Команда создания платежа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Код платежа</returns>
        public async Task<long> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var dto = _paymentMapper.CreatePaymentCommandToCreatePaymentDTO(request);
            return await _paymentService.CreatePaymentAsync(dto, cancellationToken);
        }
    }
}
