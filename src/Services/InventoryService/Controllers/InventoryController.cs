using Contracts;
using InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<InventoryResponse>> GetStock(int productId)
        {
            var result = await _inventoryService.GetStockAsync(productId);
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpPost("deduct")]
        public async Task<IActionResult> DeductStock([FromBody] DeductStockRequest request)
        {
            var success = await _inventoryService.DeductStockAsync(request.ProductId, request.Quantity);
            if (!success) return BadRequest(new { Message = "Stock insuficiente" });
            return Ok(new { Message = "Stock descontado" });
        }
    }
}
