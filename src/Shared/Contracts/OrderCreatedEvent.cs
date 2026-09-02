namespace Contracts
{
    public record OrderCreatedEvent(
        int OrderId,
        int ProductId,
        int Quantity,
        DateTime CreatedAt
    );
}
