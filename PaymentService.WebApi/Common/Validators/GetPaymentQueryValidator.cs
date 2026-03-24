using FluentValidation;
using PaymentService.WebApi.Common.Extensions;
using PaymentService.WebApi.Mediatr;

namespace PaymentService.WebApi.Common.Validators
{
    /// <summary>
    /// Валидатор запроса получения платежа
    /// </summary>
    public class GetPaymentQueryValidator : AbstractValidator<GetPaymentQuery>
    {
        /// <summary>
        /// Проверяет код платежа, должен быть положительным
        /// </summary>
        public GetPaymentQueryValidator()
        {
            RuleFor(x => x.PaymentId).ValidatePositiveNumber();
        }
    }
}
