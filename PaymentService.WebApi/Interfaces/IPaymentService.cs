using PaymentService.DataAccess.Postgres.Models.DTO;

namespace PaymentService.WebApi.Interfaces
{
    public interface IPaymentService
    {
        public Task<long> CreatePaymentAsync(CreatePaymentDTO createPaymentDTO, CancellationToken cancellationToken);
        public Task UpdatePaymentAsync(long paymentId, long statusId, CancellationToken cancellationToken);
        public Task<GetPaymentDTO> GetPaymentAsync(long paymentId, CancellationToken cancellationToken);
    }
}
