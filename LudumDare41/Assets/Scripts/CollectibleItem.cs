using UnityEngine;

public class CollectibleItem : MonoBehaviour 
{
    public ItemType itemType;
    public int qty = 1;
    private Map map;
    private MapTile mapTile;

    private void Start()
    {
        map = FindObjectOfType<Map>();
        mapTile = map.GetMapTileNearest(transform.position);
        mapTile.onArriveAtTile += OnPlayerArriveAtTile;
    }

    private void OnPlayerArriveAtTile(MapUnit mapUnit)
    {
        // is this unit actually the player?
        Player player = FindObjectOfType<Player>();
        if (Vector3.Distance(player.transform.position, transform.position) < 15.0f)
        {
            Destroy(this);
            player.Inventory.AddItem(itemType, qty);
        }
    }
}
