using Contracts;

namespace OrdersService.Services
{
    public interface IInventoryClient
    {
        public Task<InventoryResponse?> CheckStockAsync(int productId);
        public Task<bool> DeductStockAsync(int productId, int quantity);
    }
}
