using System.Collections.Generic;

public enum ItemType 
{
    Stake,
    SilverBullet,
    Key
}

public class Inventory
{
    public Dictionary<ItemType, int> Items { get; private set; }

    public Inventory()
    {
        Items = new Dictionary<ItemType, int>();
    }

    public void AddItem(ItemType itemType, int qty)
    {
        int owned = 0;
        Items.TryGetValue(itemType, out owned);
        owned += qty;
        Items[itemType] = owned;
    }
}
