using OrderService.WebApi.Common.DTO;
using Refit;

namespace OrderService.WebApi.Interfaces.ExternalApi
{
    public interface IPaymentServiceApi
    {
        [Post("/api/payments/create")]
        Task<long> CreatePaymentAsync([Body] CreatePaymentRequestDTO request, CancellationToken cancellationToken);
    }
}
