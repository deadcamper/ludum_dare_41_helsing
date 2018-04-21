using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TurnTaker
{
    private int backtrackHistorySize = 4;
    public int turnCountdown;
    public int tileCount = 1;
    private int currentCountdown;
    public MapUnit MapUnit { get; private set; }
    private List<MapTile> recentlyVisited = new List<MapTile>();

    // Use this for initialization
    void Start()
    {
        currentCountdown = turnCountdown;
        MapUnit = new MapUnit(transform.position);
    }

    public override bool TurnComplete()
    {
        return true;
    }

    public override IEnumerator TurnLogic()
    {
        if (currentCountdown <= 0)
        {
            // do a turn!
            currentCountdown = turnCountdown;

            for (int i = 0; i < tileCount; ++i)
            {
                // pick a node to move toward
                MapUnit.CurrentTile = PickATile();
                recentlyVisited.Add(MapUnit.CurrentTile);
                if (recentlyVisited.Count >= backtrackHistorySize)
                    recentlyVisited.RemoveAt(0);
                
                transform.position = MapUnit.CurrentTile.transform.position;
            }
        }

        currentCountdown--;

        yield return null;
    }

    private MapTile PickATile()
    {
        // start by considering our current tile (the current tile may have become the most beneficial tile)
        float currentMapTileScore = CalculateMapTileScore(MapUnit.CurrentTile);
        MapTile nextTile = MapUnit.CurrentTile;

        foreach (MapTile mapTile in MapUnit.CurrentTile.Neighbors)
        {
            float score = CalculateMapTileScore(mapTile);
            if (score < currentMapTileScore)
            {
                currentMapTileScore = score;
                nextTile = mapTile;
            }
        }

        return nextTile;
    }

    private float CalculateMapTileScore(MapTile mapTile)
    {
        float score = 0;

        // how close is this tile to the player?
        Player player = FindObjectOfType<Player>();
        score += Vector3.Distance(player.transform.position, mapTile.transform.position);

        // consider is this backtracking?
        if (recentlyVisited.Contains(mapTile))
        {
            score *= 1.7f;
        }

        return score;
    }
}
