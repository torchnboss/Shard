namespace Shard.Data;

public class LifemartShopImpoerter : IShopImporter
{
    public string Name => "Жизньмарт";
    public string? DefaultShopName => "Жизньмарт";

    public void Import(string content, Shop shop, Func<ShopItem, ShopItem?> addShopItem, Action<Order> addOrder,
        Action<OrderItem> addOrderItem)
    {
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var orderId = Guid.NewGuid();
        string? itemName = null;
        decimal? itemQuantity = null;
        decimal? itemPrice = null;
        decimal totalPrice = 0;
        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line == "₽") continue;

            if (itemName == null)
            {
                itemName = line;
                continue;
            }

            if (itemName == line) continue;

            if (itemQuantity == null)
            {
                itemQuantity = int.Parse(line);
                continue;
            }

            if (line == "шт") continue;

            if (itemPrice == null)
            {
                itemPrice = decimal.Parse(line.Replace(".", ","));
            }

            var shopItem = addShopItem(new ShopItem
            {
                Id = Guid.NewGuid(),
                Name = itemName,
                Price = itemPrice.Value,
                LastSync = DateTime.Now,
                ShopId = shop.Id,
            });

            if (shopItem != null)
            {
                addOrderItem(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    Name = itemName,
                    OrderId = orderId,
                    Price = itemPrice.Value,
                    Quantity = (int)itemQuantity.Value,
                    ShopItemId = shopItem.Id
                });
                totalPrice += itemPrice.Value * itemQuantity.Value;
            }

            itemName = null;
            itemQuantity = null;
            itemPrice = null;
        }

        addOrder(new Order
        {
            Id = orderId,
            ShopId = shop.Id,
            Price = totalPrice,
            CreatedAt = DateTime.Now
        });
    }
}