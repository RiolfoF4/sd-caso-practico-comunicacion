using Contracts;

namespace InventoryService.Services
{
    public interface IInventoryService
    {
        Task<InventoryResponse?> GetStockAsync(int productId);
    }
}
