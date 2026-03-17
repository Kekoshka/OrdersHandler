using MediatR;
using PaymentService.DataAccess.Postgres.Models.DTO;

namespace PaymentService.WebApi.Mediatr.Queries
{
    public class GetPaymentQuery : IRequest<GetPaymentDTO>
    {
        public long PaymentId { get; set; }
    }
}
