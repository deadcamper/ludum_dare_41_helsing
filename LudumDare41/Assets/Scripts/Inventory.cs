using System.Collections.Generic;

public enum ItemType 
{
    Stake,
    SilverBullet,
    Key
}

public class Inventory
{
    public delegate void OnItemChange(ItemType itemType, int qty);
    public event OnItemChange onItemChange;

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

        if (onItemChange != null)
            onItemChange(itemType, qty);
    }

    public bool RemoveItem(ItemType itemType, int qty)
    {
        int owned = 0;
        Items.TryGetValue(itemType, out owned);
        if (owned < qty)
            return false; // we don't have the amount we are trying to use
        
        owned -= qty;
        Items[itemType] = owned;

        if (onItemChange != null)
            onItemChange(itemType, -qty);

        return true;
    }
}
