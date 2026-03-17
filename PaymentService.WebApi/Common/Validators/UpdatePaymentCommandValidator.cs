using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr.Commands;

namespace PaymentService.WebApi.Common.Validators
{
    public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
    {
        public UpdatePaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId).ValidatePositiveNumber();
            RuleFor(x => x.StatusId).ValidatePositiveNumber();
        }
    }
}
