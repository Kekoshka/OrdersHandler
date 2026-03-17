using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentService.WebApi.Mediatr.Commands;
using PaymentService.WebApi.Mediatr.Queries;

namespace PaymentService.WebApi.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        IMediator _mediator;
        public PaymentsController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentAsync(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var paymentId = await _mediator.Send(command,cancellationToken);
            return Ok(paymentId);
        }

        [HttpPatch("updateStatus/{paymentId}/{statusId}")]
        public async Task<IActionResult> UpdatePaymentAsync(long paymentId, long statusId, CancellationToken cancellationToken)
        {
            UpdatePaymentCommand command = new() { PaymentId = paymentId, StatusId = statusId };
            await _mediator.Send(command,cancellationToken);
            return NoContent();
        }

        [HttpGet("get/{paymentId}")]
        public async Task<IActionResult> GetPaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            GetPaymentQuery query = new() { PaymentId = paymentId };
            var payment = await _mediator.Send(query, cancellationToken);
            return Ok(payment);
        }
    }
}
