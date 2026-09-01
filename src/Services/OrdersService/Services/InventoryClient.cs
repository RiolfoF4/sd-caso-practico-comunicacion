using Contracts;

namespace OrdersService.Services
{
    public class InventoryClient : IInventoryClient
    {
        private readonly HttpClient _httpClient;

        public InventoryClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<InventoryResponse?> CheckStockAsync(int productId)
        {
            return await _httpClient.GetFromJsonAsync<InventoryResponse>(
                $"/api/inventory/{productId}");
        }
    }
}
