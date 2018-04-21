using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum TileType
{
    Floor,
    Wall
}

public class MapTile : MonoBehaviour
{
    public static int TILE_SIZE = 32;
    public static bool TilesAreNeighbors(MapTile tileA, MapTile tileB) 
    {
        // are they both floors?
        if (tileA.tileType == tileB.tileType && tileB.tileType == TileType.Floor)
        {
            // get distance between the 2
            float diffX = tileA.transform.position.x - tileB.transform.position.x;
            float diffY = tileA.transform.position.y - tileB.transform.position.y;
            float dist = Mathf.Sqrt((diffX * diffX) + (diffY * diffY));
            return (dist <= TILE_SIZE);
        }

        return false;
    }

    public delegate void OnArriveAtTile(MapUnit mapUnit);
    public OnArriveAtTile onArriveAtTile;

    public TileType tileType;
    public List<MapTile> Neighbors { get; private set; }
    public void AddNeighbor(MapTile tile)
    {
        if (Neighbors == null)
            Neighbors = new List<MapTile>();

        Neighbors.Add(tile);
    }

    public MapTile GetNeighbor(Vector3 offset)
    {
        foreach (MapTile neighbor in Neighbors)
        {
            // up
            if (offset == Vector3.up)
            {
                if (neighbor.transform.position.y > transform.position.y)
                {
                    return neighbor;
                }
            }

            // down
            if (offset == Vector3.down)
            {
                if (neighbor.transform.position.y < transform.position.y)
                {
                    return neighbor;
                }
            }

            // up
            if (offset == Vector3.left)
            {
                if (neighbor.transform.position.x < transform.position.x)
                {
                    return neighbor;
                }
            }

            // down
            if (offset == Vector3.right)
            {
                if (neighbor.transform.position.x > transform.position.x)
                {
                    return neighbor;
                }
            }
        }

        return null;
    }
}
