using System.Collections.Generic;

public enum ItemType 
{
    Stake,
    SilverBullet,
    Key,
    MetalStake
}

public class Inventory
{
    public delegate void OnItemChange(ItemType itemType, int qtyDelta, int qtyTotal);
    public event OnItemChange onItemChange;

    public Dictionary<ItemType, int> Items { get; private set; }

    private static Inventory saved;
    public static void Save()
    {
        saved = instance;
    }

    public static void Load()
    {
        Clear();

        Inventory temp = GetInstance();

        if (saved != null) // if we have a saved inventory, load it
        {
            foreach (ItemType itemType in saved.Items.Keys)
            {
                temp.AddItem(itemType, saved.Items[itemType]);
            }
        }
        else
        {
            AddStartingItems();
        }
    }

    public static void Clear()
    {
        Inventory temp = GetInstance();
        foreach (ItemType itemType in temp.Items.Keys)
        {
            temp.onItemChange(itemType, -temp.Items[itemType], 0);
        }
        temp.Items.Clear();
    }

    private static Inventory instance;
    public static Inventory GetInstance()
    {
        if (instance == null)
        {
            instance = new Inventory();
            AddStartingItems();
        }

        return instance;
    }

    public static void AddStartingItems()
    {
        instance.Items.Add(ItemType.Stake, 1);
        instance.Items.Add(ItemType.SilverBullet, 0);
        instance.Items.Add(ItemType.Key, 0);
        instance.Items.Add(ItemType.MetalStake, 0);
    }

    private Inventory()
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
            onItemChange(itemType, qty, owned);
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
            onItemChange(itemType, -qty, owned);

        return true;
    }

    public bool HasItem(ItemType itemType)
    {
        int owned = 0;
        Items.TryGetValue(itemType, out owned);

        return (owned > 0);
    }
}
