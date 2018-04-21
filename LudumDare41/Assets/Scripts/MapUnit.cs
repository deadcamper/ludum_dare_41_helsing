using UnityEngine;

public class MapUnit
{
    private MapTile currentTile;
    public MapTile CurrentTile 
    { 
        get
        {
            return currentTile;
        }

        set 
        {
            currentTile = value;

            if (currentTile.onArriveAtTile != null)
            {
                currentTile.onArriveAtTile(this);
            }
        } 
    }

    public Killable Killable { get; private set; }
    private Map map;

    // Use this for initialization
    public MapUnit(Vector3 position, Killable killable)
    {
        Killable = killable;
        map = GameObject.FindObjectOfType<Map>();
        CurrentTile = map.GetMapTileNearest(position);
    }
}
