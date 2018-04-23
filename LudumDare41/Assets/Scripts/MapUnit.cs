using UnityEngine;

public class MapUnit
{
    public delegate void OnTileChange(MapTile mapTile);
    public OnTileChange onTileChange;

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
                currentTile.onArriveAtTile(value, this);
            }

            currentTile.onArriveAtTileUnity.Invoke();

            if (onTileChange != null)
                onTileChange(currentTile);
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
