using FluentValidation;
using Microsoft.Extensions.Options;
using OrderService.WebApi.Common.CustomExceptions;
using OrderService.WebApi.Common.Options;

namespace ProjectManagementSystemBackend.Middlewares
{
    /// <summary>
    /// Middleware для обработки исключений
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ContentTypesOptions _contentTypes;

        /// <summary>
        /// Инициализация нового Middleware для обработки ошибок
        /// </summary>
        /// <param name="next">Делегат следующего обработчика</param>
        /// <param name="logger">Logger</param>
        /// <param name="contentTypes">типы контента</param>
        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILogger<ExceptionHandlingMiddleware> logger,
            IOptions<ContentTypesOptions> contentTypes)
        {
            _next = next;
            _logger = logger;
            _contentTypes = contentTypes.Value;
        }
        /// <summary>
        /// Метод обработки ошибок, если в последующих методах выбросится исключение, оно будет обработано
        /// </summary>
        /// <param name="context">HttpContext</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred: {message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                Exception => StatusCodes.Status500InternalServerError
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = _contentTypes.Json;

            var response = new
            {
                error = statusCode == StatusCodes.Status500InternalServerError ? "Internal server error" : exception.Message,
                status = statusCode
            };
            
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
