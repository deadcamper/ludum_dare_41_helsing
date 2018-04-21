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
}
