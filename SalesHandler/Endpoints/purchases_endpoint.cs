using Microsoft.AspNetCore.Mvc;
using SalesService.Handlers;
namespace SalesService.Endpoints
{
    [ApiController]
    [Route("purchases")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromHeader(Name = "user_id")] Guid userId, [FromBody] CreateOrderRequest payload)
        {
            var purchase = await _orderService.CreateOrderAsync(userId, payload.Amount, payload.Description);
            return CreatedAtAction(nameof(GetOrder), new { orderId = purchase.Id }, new { purchase.Id, purchase.Status });
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromHeader(Name = "user_id")] Guid userId)
        {
            var purchases = await _orderService.GetOrdersAsync(userId);
            return Ok(purchases.Select(o => new { o.Id, o.Status, o.Amount, o.Description, o.CreatedAt }));
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(Guid orderId, [FromHeader(Name = "user_id")] Guid userId)
        {
            var purchase = await _orderService.GetOrderByIdAsync(orderId);
            if (purchase == null || purchase.UserId != userId)
                return NotFound();
            return Ok(new { purchase.Id, purchase.Status, purchase.Amount, purchase.Description, purchase.CreatedAt });
        }
    }
    public record CreateOrderRequest(decimal Amount, string Description);
}