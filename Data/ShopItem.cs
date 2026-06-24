namespace Shard.Data;

public class ShopItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public Guid ShopId { get; set; }
    public DateTime LastSync { get; set; }
}