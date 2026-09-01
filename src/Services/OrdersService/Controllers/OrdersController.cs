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

    }
}
