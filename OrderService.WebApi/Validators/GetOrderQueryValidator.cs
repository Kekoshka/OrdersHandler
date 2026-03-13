using FluentValidation;
using OrderService.WebApi.Extensions;
using OrderService.WebApi.Mediatr.Queries;

namespace OrderService.WebApi.Validators
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
