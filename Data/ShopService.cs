using LiteDB;

namespace Shard.Data;

public class ShopService(GlobalData data)
{
    public Shop? GetShopByName(string shopName)
    {
        if (data.ActivePath == null) return null;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Shop>("shops");

        col.EnsureIndex(x => x.Name);

        var result = col.Query()
            .Where(x => x.Name == shopName)
            .FirstOrDefault();

        return result;
    }

    public ShopItem? UpsertShopItem(ShopItem item)
    {
        if (data.ActivePath == null) return null;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<ShopItem>("shop_items");

        var result = col.Query()
            .Where(x => x.Name == item.Name && x.ShopId == item.ShopId)
            .FirstOrDefault();

        if (result == null)
        {
            col.Insert(item);
            return item;
        }

        if (result.Price == item.Price) return result;
        result.Price = item.Price;
        result.LastSync = DateTime.Now;
        col.Update(result);

        return result;
    }

    public void InsertOrder(Order order)
    {
        if (data.ActivePath == null) return;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Order>("orders");
        col.Insert(order);
    }

    public void InsertOrderItem(OrderItem item)
    {
        if (data.ActivePath == null) return;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<OrderItem>("order_items");
        col.Insert(item);
    }

    public List<Shop> GetShopsByImporter(string shopImporterName)
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Shop>("shops");

        col.EnsureIndex(x => x.Name);

        var results = col.Query()
            .Where(x => x.ImporterName == shopImporterName)
            .OrderBy(x => x.Name)
            .ToList();

        return results;
    }

    public List<Shop> GetShops()
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Shop>("shops");

        col.EnsureIndex(x => x.Name);

        var results = col.Query()
            .OrderBy(x => x.Name)
            .ToList();

        return results;
    }

    public List<Order> GetOrders()
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Order>("orders");

        var results = col.Query()
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return results;
    }

    public List<OrderItem> GetOrderItems(Order order)
    {
        if (data.ActivePath == null) return [];

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<OrderItem>("order_items");

        var results = col.Query()
            .Where(x => x.OrderId == order.Id)
            .OrderBy(x => x.Name)
            .ToList();

        return results;
    }

    public Shop? CreateShop(string shopName, string shopImporterName)
    {
        if (data.ActivePath == null) return null;

        using var db = new LiteDatabase(data.ActivePath);
        var col = db.GetCollection<Shop>("shops");

        var shop = new Shop
        {
            Id = Guid.NewGuid(),
            Name = shopName,
            ImporterName = shopImporterName
        };

        col.Insert(shop);

        return shop;
    }
}