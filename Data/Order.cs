namespace Shard.Data;

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ShopId { get; set; }
    public decimal Price { get; set; }
}