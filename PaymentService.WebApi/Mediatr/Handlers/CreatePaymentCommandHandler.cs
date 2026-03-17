using MediatR;
using PaymentService.WebApi.Common.Mappers;
using PaymentService.WebApi.Interfaces;
using PaymentService.WebApi.Mediatr.Commands;

namespace PaymentService.WebApi.Mediatr.Handlers
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, long>
    {
        IPaymentService _paymentService;
        PaymentMapper _paymentMapper;
        public CreatePaymentCommandHandler(IPaymentService paymentService,
            PaymentMapper paymentMapper)
        {
            _paymentService = paymentService;
            _paymentMapper = paymentMapper;
        }
        public async Task<long> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var dto = _paymentMapper.CreatePaymentCommandToCreatePaymentDTO(request);
            return await _paymentService.CreatePaymentAsync(dto, cancellationToken);
        }
    }
}
