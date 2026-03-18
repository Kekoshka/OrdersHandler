using FluentValidation;
using MediatR;

namespace OrderService.WebApi.Common.Behaviors
{
    /// <summary>
    /// Выполняет валидацию входящего запроса с помощью всех зарегистрированных валидаторов,
    /// и в случае обнаружения ошибок выбрасывает исключение ValidationException.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса, который должен реализовывать интерфейс IRequest<TResponse></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Инициализирует новый экземпляр поведения с переданной коллекцией валидаторов.
        /// </summary>
        /// <param name="validators">Валидаторы, которые будут применены к запросу</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => 
            _validators = validators;

        /// <summary>
        /// Выполняет валидацию, если ошибок нет, передаёт управление следующему элементу пайплайна.
        /// </summary>
        /// <param name="request">Входящий запрос</param>
        /// <param name="next">Делегат следующего обработчика в пайплайне</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат выполнения запроса</returns>
        /// <exception cref="ValidationException">Выбрасывается, если есть хотя бы одна ошибка валидации</exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any()) throw new ValidationException(failures);
            return await next();
        }
    }
}
