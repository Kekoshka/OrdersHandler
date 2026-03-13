using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.WebApi.Mediatr.Commands;
using OrderService.WebApi.Mediatr.Queries;

namespace OrderService.WebApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с заказами
    /// Предоставляет методы для создания, получения и удаления заказов
    /// </summary>
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IMediator _mediator;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера заказов.
        /// </summary>
        /// <param name="mediator">Медиатор для отправки команд/запросов</param>
        public OrdersController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Создает новый заказ
        /// </summary>
        /// <param name="command">Команда создания заказа, содержащая необходимые данные</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Id созданного заказа</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var orderId = await _mediator.Send(command);
            return Ok(orderId);
        }

        /// <summary>
        /// Возвращает данные заказа по Id
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Данные заказа</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderAsync(long orderId, CancellationToken cancellationToken)
        {
            GetOrderQuery query = new() { OrderId = orderId };
            var order = await _mediator.Send(query);
            return Ok(order);
        }

        /// <summary>
        /// Удаляет заказ по Id
        /// </summary>
        /// <param name="id">Id заказа</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Статус 204 NoContent при успешном удалении</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteOrderAsync(long id, CancellationToken cancellationToken)
        {
            DeleteOrderCommand command = new() { OrderId = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
