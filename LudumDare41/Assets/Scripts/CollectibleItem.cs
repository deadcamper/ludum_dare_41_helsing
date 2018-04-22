using UnityEngine;
using UnityEngine.Events;

public class CollectibleItem : MonoBehaviour 
{
    
    public ItemType itemType;
    public int qty = 1;
    private Map map;
    private MapTile currentMapTile;

    public UnityEvent OnCollect;

    private void Start()
    {
        map = FindObjectOfType<Map>();
        currentMapTile = map.GetMapTileNearest(transform.position);
        currentMapTile.onArriveAtTile += OnPlayerArriveAtTile;
    }

    private void OnPlayerArriveAtTile(MapTile mapTile, MapUnit mapUnit)
    {
        // is this unit actually the player?
        Player player = FindObjectOfType<Player>();
        if (mapUnit == player.MapUnit)
        {
            Destroy(gameObject);
            Inventory.GetInstance().AddItem(itemType, qty);
            mapTile.onArriveAtTile -= OnPlayerArriveAtTile;
            OnCollect.Invoke();
        }
    }
}
