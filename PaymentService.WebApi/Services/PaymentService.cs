using PaymentService.DataAccess.Postgres.Context;
using PaymentService.WebApi.Interfaces;
using PaymentService.WebApi.Common.Mappers;
using Microsoft.Extensions.Options;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Common.DTO;
using ExceptionHandler.Exceptions;

namespace PaymentService.WebApi.Services
{
    /// <summary>
    /// Сервис обработки платежей
    /// </summary>
    public class PaymentService : IPaymentService
    {
        /// <summary>
        /// Контекст базы данных
        /// </summary>
        AppDbContext _context;

        /// <summary>
        /// Маппер платежей
        /// </summary>
        PaymentMapper _paymentMapper;
        
        /// <summary>
        /// Настройки статусов платежей
        /// </summary>
        StatusesOptions _statuses;

        /// <summary>
        /// Сервис общения с Kafka
        /// </summary>
        IKafkaService _kafkaService;

        /// <summary>
        /// Настройки внешних сервисов
        /// </summary>
        ExternalServicesOptions _externalServicesOptions;
        public PaymentService(AppDbContext context,
            PaymentMapper paymentMapper,
            IOptions<StatusesOptions> statuses,
            IKafkaService kafkaService,
            IOptions<ExternalServicesOptions> externalServicesOptions)
        {
            _context = context;
            _paymentMapper = paymentMapper;
            _statuses = statuses.Value;
            _kafkaService = kafkaService;
            _externalServicesOptions = externalServicesOptions.Value;
        }

        /// <summary>
        /// Создание нового платежа
        /// </summary>
        /// <param name="createPaymentDTO">DTO с данными нового платежа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Код платежа</returns>
        public async Task<long> CreatePaymentAsync(CreatePaymentDTO createPaymentDTO, CancellationToken cancellationToken)
        {
            var payment = _paymentMapper.GetPaymentDTOToPayment(createPaymentDTO);
            payment.Status = false;
            payment.DateCreate = DateTime.UtcNow;
            await _context.AddAsync(payment,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return payment.Id;
        }

        /// <summary>
        /// Обновление статуса платежа
        /// </summary>
        /// <remarks>
        /// 0 - не оплачено
        /// 1 - оплачено
        /// </remarks>
        /// <param name="paymentId">Код платежа</param>
        /// <param name="status">Статус платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public async Task UpdatePaymentAsync(long paymentId, bool status, CancellationToken cancellationToken)
        {
           var payment = await _context.Payments.FindAsync(paymentId, cancellationToken);
            if (payment is null)
                throw new NotFoundException($"Payment with id {paymentId} not found");

            payment.Status = status;

            await _context.SaveChangesAsync(cancellationToken);

            var message = _paymentMapper.PaymentToPaymentUpdated(payment);
            await _kafkaService.ProduceAsync(_externalServicesOptions.PaymentServiceTopic,
                message, 
                payment.Id, 
                cancellationToken);  
        }

        /// <summary>
        /// Получение DTO платежа по коду 
        /// </summary>
        /// <param name="paymentId">Код платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO платежа</returns>
        /// <exception cref="NotFoundException">Выбрасывается если платеж с указанным кодом не найден</exception>
        public async Task<GetPaymentDTO> GetPaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FindAsync(paymentId,cancellationToken);
            return payment is not null ?
                _paymentMapper.PaymentToGetPaymentDTO(payment)
                : throw new NotFoundException($"Payment with id {paymentId} not found");
        }
    }
}
