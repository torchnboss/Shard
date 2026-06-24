namespace Shard.Data;

public class LifemartShopImpoerter : IShopImporter
{
    public string Name => "Жизньмарт";
    public string? DefaultShopName => "Жизньмарт";
    public void Import(string content, Shop shop, Func<ShopItem, ShopItem?> addShopItem, Action<Order> addOrder, Action<OrderItem> addOrderItem)
    {
        
    }
}