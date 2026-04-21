using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentService.WebApi.Common.Options;
using PaymentService.WebApi.Mediatr;

namespace PaymentService.WebApi.Controllers
{

    /// <summary>
    /// Управляет платежами
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        /// <summary>
        /// Медиатор
        /// </summary>
        IMediator _mediator;
        
        /// <summary>
        /// Настройки статусов платежей
        /// </summary>
        StatusesOptions _statuses;
        public PaymentsController(IMediator mediator,
            IOptions<StatusesOptions> statusesOptions) 
        {
            _mediator = mediator;
            _statuses = statusesOptions.Value;
        }

        /// <summary>
        /// Создать новый платеж
        /// </summary>
        /// <param name="command">Команда создания платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Код созданного платежа</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePaymentAsync(CreatePaymentCommand command, CancellationToken cancellationToken)
        {
            var paymentId = await _mediator.Send(command,cancellationToken);
            return Ok(paymentId);
        }

        [HttpPatch("updateStatus/{paymentId}/{status}")]
        public async Task<IActionResult> UpdatePaymentAsync(long paymentId, long status, CancellationToken cancellationToken)
        {
            UpdatePaymentCommand command = new() 
            {
                PaymentId = paymentId,
                Status = status == _statuses.Completed };
            await _mediator.Send(command,cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Получить объект платежа по Id
        /// </summary>
        /// <param name="paymentId">Код платежа</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO платежа</returns>
        [HttpGet("get/{paymentId}")]
        public async Task<IActionResult> GetPaymentAsync(long paymentId, CancellationToken cancellationToken)
        {
            GetPaymentQuery query = new() { PaymentId = paymentId };
            var payment = await _mediator.Send(query, cancellationToken);
            return Ok(payment);
        }
    }
}
