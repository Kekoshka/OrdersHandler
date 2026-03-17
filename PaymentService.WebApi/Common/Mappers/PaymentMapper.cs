using Microsoft.Extensions.Options;
using PaymentService.DataAccess.Postgres.Models;
using PaymentService.DataAccess.Postgres.Models.DTO;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Mediatr.Commands;
using Riok.Mapperly.Abstractions;

namespace PaymentService.WebApi.Common.Mappers
{
    [Mapper]
    public partial class PaymentMapper
    {
        StatusesOptions _statusesOptions { get; set; }
        public PaymentMapper(IOptions<StatusesOptions> statuses) 
        {
            _statusesOptions = statuses.Value;
        }

        [MapProperty(nameof(Payment.StatusId),nameof(GetPaymentDTO.Status))]
        public partial GetPaymentDTO PaymentToGetPaymentDTO(Payment payment);
        public partial Payment GetPaymentDTOToPayment(CreatePaymentDTO createPaymentDTO);
        public partial CreatePaymentDTO CreatePaymentCommandToCreatePaymentDTO(CreatePaymentCommand createPaymentCommand);
        
        private bool longToBool(long value) => value == _statusesOptions.CompletedId;
    }
}
