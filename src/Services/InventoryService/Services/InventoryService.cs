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

        public async Task<bool> DeductStockAsync(int productId, int quantity)
        {
            Product? product = await _inventoryDbContext.Products.FindAsync(productId);
            if (product is null || product.Stock < quantity) return false;

            product.Stock -= quantity;
            await _inventoryDbContext.SaveChangesAsync();
            return true;
        }
    }
}
