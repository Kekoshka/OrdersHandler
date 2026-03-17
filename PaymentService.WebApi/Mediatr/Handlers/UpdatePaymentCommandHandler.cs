using MediatR;
using PaymentService.WebApi.Common.Mappers;
using PaymentService.WebApi.Interfaces;
using PaymentService.WebApi.Mediatr.Commands;

namespace PaymentService.WebApi.Mediatr.Handlers
{
    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand>
    {
        IPaymentService _paymentService;

        public UpdatePaymentCommandHandler(IPaymentService paymentService,
            PaymentMapper paymentMapper)
        {
            _paymentService = paymentService;
        }
        public async Task Handle(UpdatePaymentCommand request, CancellationToken cancellationToken) =>
            await _paymentService.UpdatePaymentAsync(request.PaymentId, request.StatusId, cancellationToken);
    }
}
