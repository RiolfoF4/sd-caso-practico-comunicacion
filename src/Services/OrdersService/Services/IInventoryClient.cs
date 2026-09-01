using Contracts;

namespace OrdersService.Services
{
    public interface IInventoryClient
    {
        public Task<InventoryResponse?> CheckStockAsync(int productId);
    }
}
