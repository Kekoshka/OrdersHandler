using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr;

namespace PaymentService.WebApi.Common.Validators
{
    /// <summary>
    /// Валидатор команды создания платежа
    /// </summary>
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        /// <summary>
        /// Проверяет цену и код заказа на положительность значения
        /// </summary>
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).ValidatePositiveNumber();
            RuleFor(x => x.Price).ValidatePositiveNumber();
        }
    }
}
