using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public float light_radius = 130.0f;
    public float enemy_light_radius = -1f;


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

    [FormerlySerializedAs("wallStyle")]
    public WallStyle baseWallStyle;

	public void AddMapTile(MapTile mapTile)
	{
		mapTiles[mapTile.Coordinates] = mapTile;
	}

	float lastUpdatedMapTileLookup;

#if UNITY_EDITOR
	[UnityEditor.MenuItem("Rebuild/Rebuild")]
#endif
	public void ForceRebuildMapInEditor()
	{
		lastUpdatedMapTileLookup = Time.realtimeSinceStartup;
		mapTiles.Clear();
		MapTile[] tiles = FindObjectsOfType<MapTile>();
		foreach (MapTile tile in tiles)
		{
			mapTiles[tile.Coordinates] = tile;
		}
		foreach (MapTile tile in tiles)
		{
			tile.RegenerateDecorations(this);
		}
	}
	public void RebuildMapInEditor()
	{
		if (!Application.isPlaying)
		{
			if (Time.realtimeSinceStartup - lastUpdatedMapTileLookup > 1)
			{
				ForceRebuildMapInEditor();
			}
		}
	}

	public MapTile GetMapTile(Vector2Int coordinates)
	{
		RebuildMapInEditor();

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

#if UNITY_EDITOR
		UnityEditor.EditorApplication.update -= RebuildMapInEditor;
		UnityEditor.EditorApplication.update += RebuildMapInEditor;
#endif

		if (Application.isPlaying)
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
					}
				}
			}
		}
    }

    private void OnArrivedAtExit(MapTile mapTile, MapUnit mapUnit)
    {
        // is this unit actually the player?
        Player player = FindObjectOfType<Player>();
        if (mapUnit == player.MapUnit)
        {
            // remove this so it doesn't keep happening
            mapTile.onArriveAtTile -= OnArrivedAtExit;

            // Win the game!
            Debug.Log("Win the game!");
            player.PlayClip("win");
        }
    }

    // Update is called once per frame
    void Update()
    {
		if (Application.isPlaying)
		{
			Player player = FindObjectOfType<Player>();
			MapTile[] mapTiles = GetComponentsInChildren<MapTile>();
			Enemy[] enemies = FindObjectsOfType<Enemy>();
			foreach (MapTile mapTile in mapTiles)
			{
				SpriteRenderer sr = mapTile.GetComponent<SpriteRenderer>();
				sr.color = Color.black;

				sr.color += Color.Lerp(Color.white, Color.clear, Vector3.Distance(mapTile.transform.position, player.transform.position) / light_radius);

				if (enemy_light_radius > 0f)
				{
					foreach (Enemy enemy in enemies)
					{
						sr.color += Color.Lerp(Color.white, Color.clear, Vector3.Distance(mapTile.transform.position, enemy.transform.position) / enemy_light_radius);
					}
				}

				SpriteRenderer[] renderers = mapTile.GetComponentsInChildren<SpriteRenderer>();
				foreach (SpriteRenderer ren in renderers)
				{
					ren.color = sr.color;
				}
			}
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
	void OnDestroy()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.update -= RebuildMapInEditor;
#endif
	}
}
