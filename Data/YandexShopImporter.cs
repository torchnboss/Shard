namespace Shard.Data;

public class YandexShopImporter : IShopImporter
{
    public string Name => "Yandex Delivery";

    public string? DefaultShopName => null;

    private const string Numbers = "0123456789.,";

    private static decimal ParseNumber(string number)
        => decimal.Parse(string.Join("", number.Where(x => Numbers.Contains(x)).ToArray()).Replace(".", ","));

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
            
            if (itemName == null)
            {
                itemName = line;
                continue;
            }

            if (itemQuantity == null)
            {
                itemQuantity = ParseNumber(line);
                continue;
            }

            if (!line.Contains("шт")) continue;

            if (itemPrice == null)
            {
                itemPrice = ParseNumber(line);
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