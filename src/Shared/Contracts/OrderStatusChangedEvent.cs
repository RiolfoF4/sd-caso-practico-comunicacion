namespace Contracts
{
    public record OrderStatusChangedEvent(
        int OrderId,
        int ProductId,
        int Quantity,
        OrderStatus Status,
        DateTime OccurredAt
     );
}
