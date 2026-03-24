using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr;

namespace PaymentService.WebApi.Common.Validators
{
    /// <summary>
    /// Валидатор команды обновления платежа
    /// </summary>
    public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
    {
        /// <summary>
        /// Код платежа должен быть положительным числом
        /// </summary>
        public UpdatePaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId).ValidatePositiveNumber();
        }
    }
}
