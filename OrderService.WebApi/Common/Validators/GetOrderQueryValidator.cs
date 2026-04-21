using FluentValidation;
using OrderService.WebApi.Common.Extensions;
using OrderService.WebApi.Mediatr;

namespace OrderService.WebApi.Common.Validators
{
    /// <summary>
    /// Валидатор запросов получения заказов
    /// </summary>
    public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
    {
        /// <summary>
        /// Валидирует запросы получения заказов
        /// </summary>
        public GetOrderQueryValidator() 
        {
            RuleFor(x => x.OrderId).ValidatePositiveNumber();
        }
    }
}
