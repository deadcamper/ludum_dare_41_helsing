using UnityEngine;

public class CollectibleItem : MonoBehaviour 
{
    public ItemType itemType;
    private Map map;
    private MapTile mapTile;

    private void Start()
    {
        map = FindObjectOfType<Map>();
        mapTile = map.GetMapTileNearest(transform.position);
    }
}
