using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr.Commands;

namespace PaymentService.WebApi.Common.Validators
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.OrderId).ValidatePositiveNumber();
            RuleFor(x => x.Price).ValidatePositiveNumber();
        }
    }
}
