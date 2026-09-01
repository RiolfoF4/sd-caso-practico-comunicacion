using Contracts;
using InventoryService.Data;
using InventoryService.Models;

namespace InventoryService.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly InventoryDbContext _inventoryDbContext;

        public InventoryService(InventoryDbContext inventoryDbContext)
        {
            _inventoryDbContext = inventoryDbContext;
        }

        public async Task<InventoryResponse?> GetStockAsync(int productId)
        {
            Product? product = await _inventoryDbContext.Products.FindAsync(productId);
            if (product is null) return null;

            return new InventoryResponse(product.Id, product.Stock, product.Stock > 0);
        }
    }
}
