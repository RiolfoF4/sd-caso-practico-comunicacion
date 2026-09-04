using Contracts;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Services;

namespace OrdersService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] OrderRequest request)
        {
            var result = await _ordersService.CreateOrderAsync(request);
            if (result is null)
                return BadRequest(new { Message = "Stock no disponible" });

            return CreatedAtAction(nameof(CreateOrder), new { id = result.OrderId }, result);
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult<OrderResponse>> ConfirmOrder(int id)
        {
            var result = await _ordersService.ConfirmOrderAsync(id);
            if (result is null)
                return BadRequest(new { Message = "Pedido no encontrado o transición inválida" });

            return Ok(result);
        }

        [HttpPost("{id}/ship")]
        public async Task<ActionResult<OrderResponse>> ShipOrder(int id)
        {
            var result = await _ordersService.ShipOrderAsync(id);
            if (result is null)
                return BadRequest(new { Message = "Pedido no encontrado o transición inválida" });

            return Ok(result);
        }
    }
}
