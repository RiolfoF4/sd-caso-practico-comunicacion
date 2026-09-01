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
    }
}
