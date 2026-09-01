namespace Contracts
{
    public record InventoryResponse(
        int ProductId,
        int Stock,
        bool IsAvailable
    );
}
