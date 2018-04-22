using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    private static float light_radius = 130.0f;

	private static Map _instance;
	public static Map Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Map>();
			}
			return _instance;
		}
	}

	private Dictionary<Vector2Int, MapTile> mapTiles = new Dictionary<Vector2Int, MapTile>();

	public void AddMapTile(MapTile mapTile)
	{
		mapTiles[mapTile.Coordinates] = mapTile;
	}

	public MapTile GetMapTile(Vector2Int coordinates)
	{
		//if (!Application.isPlaying)
		{
			return FindObjectsOfType<MapTile>().FirstOrDefault(t => t.Coordinates == coordinates);
		}



		MapTile mapTile;
		if (mapTiles.TryGetValue(coordinates, out mapTile))
		{
			return mapTile;
		}
		return null;
	}

    // Use this for initialization
    void Awake()
    {
        MapTile[] mapTiles = GetComponentsInChildren<MapTile>();
        int count = mapTiles.Length;

        for (int i = 0; i < count; ++i)
        {
            for (int j = 0; j < count; ++j)
            {
                if (i != j) 
                {
                    MapTile tileA = mapTiles[i];

                    if (tileA.tileType == TileType.Exit)
                    {
                        tileA.onArriveAtTile += OnArrivedAtExit;
                    }

                    MapTile tileB = mapTiles[j];

                    if (MapTile.TilesAreNeighbors(tileA, tileB))
                    {
                        tileA.AddNeighbor(tileB);
                    }
                }
            }
        }
    }

    private void OnArrivedAtExit(MapTile mapTile, MapUnit mapUnit)
    {
        // remove this so it doesn't keep happening
        mapTile.onArriveAtTile -= OnArrivedAtExit;

        // is this unit actually the player?
        Player player = FindObjectOfType<Player>();
        if (player.MapUnit.CurrentTile == mapTile)
        {
            // Win the game!
            Debug.Log("Win the game!");
            player.PlayClip("win", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Player player = FindObjectOfType<Player>();
        MapTile[] mapTiles = GetComponentsInChildren<MapTile>();
        foreach (MapTile mapTile in mapTiles)
        {
            SpriteRenderer sr = mapTile.GetComponent<SpriteRenderer>();
            sr.color = Color.Lerp(Color.white, Color.clear, Vector3.Distance(mapTile.transform.position, player.transform.position) / light_radius);
        }
    }

    public MapTile GetMapTileNearest(Vector3 currentPos)
    {
        MapTile tMin = null;
        float minDist = Mathf.Infinity;

        MapTile[] mapTiles = GetComponentsInChildren<MapTile>();
        foreach (MapTile t in mapTiles)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        return tMin;
    }
}
