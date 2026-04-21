using OrderService.WebApi.Common.DTO;
using Refit;

namespace OrderService.WebApi.Common.ExternalApi
{
    public interface IPaymentServiceApi
    {
        [Post("/api/payments/create")]
        Task<long> CreatePaymentAsync([Body] CreatePaymentRequestDTO request, CancellationToken cancellationToken);
    }
}
