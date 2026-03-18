using Microsoft.EntityFrameworkCore;
using PaymentService.DataAccess.Postgres.Context;
using PaymentService.DataAccess.Postgres.Models.DTO;
using PaymentService.WebApi.Interfaces;
using PaymentService.WebApi.Common.CustomExceptions;
using PaymentService.WebApi.Common.Mappers;
using Microsoft.Extensions.Options;
using PaymentService.WebApi.Common.Options;

namespace PaymentService.WebApi.Services
{
    public class PaymentService : IPaymentService
    {
        AppDbContext _context;
        PaymentMapper _paymentMapper;
        StatusesOptions _statuses;
        IKafkaService _kafkaService;
        public PaymentService(AppDbContext context,
            PaymentMapper paymentMapper,
            IOptions<StatusesOptions> statuses,
            IKafkaService kafkaService)
        {
            _context = context;
            _paymentMapper = paymentMapper;
            _statuses = statuses.Value;
            _kafkaService = kafkaService;
        }
        public async Task<long> CreatePaymentAsync(CreatePaymentDTO createPaymentDTO, CancellationToken cancellationToken)
        {
            var payment = _paymentMapper.GetPaymentDTOToPayment(createPaymentDTO);
            payment.DateCreate = DateTime.UtcNow;
            payment.StatusId = _statuses.InProgressId;
            await _context.AddAsync(payment,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return payment.Id;
        }
        public async Task UpdatePaymentAsync(long paymentId, long statusId, CancellationToken cancellationToken)
        {
            var updatedPayments = await _context.Payments
                .Where(p => p.Id == paymentId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.StatusId, statusId), cancellationToken);
            if (updatedPayments == 0)
                throw new NotFoundException($"Payment with id {paymentId} not found");
            if (statusId == _statuses.CompletedId)
                await _kafkaService.ProduceAsync("paymentService", "paymentComplete", cancellationToken);
        }
        public async Task<GetPaymentDTO> GetPaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FindAsync(paymentId,cancellationToken);
            return payment is not null ?
                _paymentMapper.PaymentToGetPaymentDTO(payment)
                : throw new NotFoundException($"Payment with id {paymentId} not found");
        }
    }
}
