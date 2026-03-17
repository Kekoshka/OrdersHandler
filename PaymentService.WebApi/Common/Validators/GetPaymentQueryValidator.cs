using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr.Queries;

namespace PaymentService.WebApi.Common.Validators
{
    public class GetPaymentQueryValidator : AbstractValidator<GetPaymentQuery>
    {
        public GetPaymentQueryValidator()
        {
            RuleFor(x => x.PaymentId).ValidatePositiveNumber();
        }
    }
}
