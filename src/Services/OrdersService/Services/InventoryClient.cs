using System.Net.Http.Json;
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

        public async Task<bool> DeductStockAsync(int productId, int quantity)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/inventory/deduct",
                new DeductStockRequest(productId, quantity));
            return response.IsSuccessStatusCode;
        }
    }
}
