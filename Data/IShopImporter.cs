namespace Shard.Data;

public interface IShopImporter
{
    public string Name { get; }
    
    public string? DefaultShopName { get; }

    public void Import(string content, Shop shop, Func<ShopItem, ShopItem?> addShopItem, Action<Order> addOrder,
        Action<OrderItem> addOrderItem);
}