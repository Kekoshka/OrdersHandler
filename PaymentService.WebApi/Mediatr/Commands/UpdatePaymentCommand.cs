using MediatR;

namespace PaymentService.WebApi.Mediatr.Commands
{
    public class UpdatePaymentCommand : IRequest
    {
        public long PaymentId { get; set; }
        public long StatusId { get; set; }
    }
}
