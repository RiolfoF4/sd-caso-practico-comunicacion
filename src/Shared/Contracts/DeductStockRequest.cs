namespace Contracts
{
    public record DeductStockRequest(
        int ProductId,
        int Quantity
    );
}
