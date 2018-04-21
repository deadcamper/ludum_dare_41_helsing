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

    private void OnPlayerArriveAtTile(MapTile mapTile, MapUnit mapUnit)
    {
        // is this unit actually the player?
        Player player = FindObjectOfType<Player>();
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist < 40.0f) // why 40? because 32 won't work because the player isn't actually there yet until next frame
        {
            Destroy(gameObject);
            player.Inventory.AddItem(itemType, qty);
        }

        mapTile.onArriveAtTile -= OnPlayerArriveAtTile;
    }
}
