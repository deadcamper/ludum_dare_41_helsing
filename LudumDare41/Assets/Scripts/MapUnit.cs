using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUnit
{
    public MapTile CurrentTile { get; set; }
    private Map map;

    // Use this for initialization
    public MapUnit(Vector3 position)
    {
        map = GameObject.FindObjectOfType<Map>();
        CurrentTile = map.GetMapTileNearest(position);
    }
}
