namespace Contracts
{
    public record OrderResponse(
        int OrderId,
        int ProductId,
        int Quantity,
        OrderStatus Status,
        DateTime CreatedAt
    );
}
