using MediatR;
using PaymentService.DataAccess.Postgres.Models.DTO;
using PaymentService.WebApi.Interfaces;
using PaymentService.WebApi.Mediatr.Queries;
using PaymentService.WebApi.Services;

namespace PaymentService.WebApi.Mediatr.Handlers
{
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, GetPaymentDTO>
    {
        IPaymentService _paymentService;
        public GetPaymentQueryHandler(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public async Task<GetPaymentDTO> Handle(GetPaymentQuery request, CancellationToken cancellationToken) =>
            await _paymentService.GetPaymentAsync(request.PaymentId,cancellationToken);
    }
}
