using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public enum TileType
{
    Floor,
    Wall,
    Door,
    Exit
}

[ExecuteInEditMode]
public class MapTile : MonoBehaviour
{
    public static int TILE_SIZE = 32;

	SpriteRenderer _spriteRenderer;
	SpriteRenderer SpriteRenderer
	{
		get
		{
			if (_spriteRenderer == null)
			{
				_spriteRenderer = GetComponent<SpriteRenderer>();
			}
			return _spriteRenderer;
		}
	}
	Transform _generatedDecorationsParent;
	Transform GeneratedDecorationsParent
	{
		get
		{
			if (_generatedDecorationsParent == null)
			{
				foreach (Transform childT in transform)
				{
					if (childT.name.Contains("GeneratedDecorationsParent"))
					{
						if (_generatedDecorationsParent == null)
						{
							_generatedDecorationsParent = childT;
						}
						else
						{
							/*if (!Application.isPlaying)
								DestroyImmediate(childT.gameObject);
							else
								Destroy(childT.gameObject);*/
						}
					}
				}
				if (_generatedDecorationsParent == null)
				{
					if (Application.isPlaying)
					{
						GameObject temp = new GameObject("GeneratedDecorationsParent");
						_generatedDecorationsParent = Instantiate(temp).transform;
						Destroy(temp);
					}
					else
					{
						_generatedDecorationsParent = new GameObject("GeneratedDecorationsParent").transform;
					}
				}
			}
			_generatedDecorationsParent.SetParent(transform, false);
			_generatedDecorationsParent.gameObject.hideFlags = HideFlags.HideAndDontSave;
			return _generatedDecorationsParent;
		}
	}

	void Awake()
	{
		Map.Instance.AddMapTile(this);
	}
	void Start()
	{
		RegenerateDecorations(Map.Instance);
	}

	public void RegenerateDecorations(Map map)
	{
        WallStyle wallStyle = (this.wallStyle == null) ? map.baseWallStyle : this.wallStyle;

        if (wallStyle == null)
        {
            Debug.LogError("Wall style is undefined in Map. Define a wall style");
            return;
        }

        if (tileType == TileType.Wall || tileType == TileType.Door)
        {
            SpriteRenderer.sprite = wallStyle.fill;
        }
        else
        {
            SpriteRenderer.sprite = wallStyle.floor;
        }

		if (!Application.isPlaying)
			DestroyImmediate(GeneratedDecorationsParent.gameObject);
		else
			foreach (Transform child in GeneratedDecorationsParent)
			{
				Destroy(child.gameObject);
			}

		if (wallStyle != null)
		{
			var spriteInfos = wallStyle.GetWallSprites(map, Coordinates);
			foreach (var spriteInfo in spriteInfos)
			{
				AddGeneratedDecoratorSprite(spriteInfo.sprite, spriteInfo.rotation, spriteInfo.sortingLayer, spriteInfo.orderInLayer);
			}
		}
	}
	private void AddGeneratedDecoratorSprite(Sprite sprite, Quaternion rotation, SortingLayer sortingLayer, int orderInLayer)
	{
		GameObject newGameObject =  new GameObject("generated");
		newGameObject.transform.SetParent(GeneratedDecorationsParent);
		newGameObject.transform.localPosition = Vector3.zero;
		newGameObject.transform.localScale = Vector3.one;
		newGameObject.transform.rotation = rotation;
		SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		spriteRenderer.sortingLayerName = sortingLayer.name;
		spriteRenderer.sortingOrder = orderInLayer;
	}

	public Vector2Int Coordinates
	{
		get
		{
			return new Vector2Int(Mathf.RoundToInt(transform.position.x / TILE_SIZE), Mathf.RoundToInt(transform.position.y / TILE_SIZE));
		}
	}

    public static bool TilesAreNeighbors(MapTile tileA, MapTile tileB) 
    {
        // are they both not walls?
        if (tileA.tileType != TileType.Wall && tileB.tileType != TileType.Wall)
        {
            // get distance between the 2
            float diffX = tileA.transform.position.x - tileB.transform.position.x;
            float diffY = tileA.transform.position.y - tileB.transform.position.y;
            float dist = Mathf.Sqrt((diffX * diffX) + (diffY * diffY));
            return (dist <= TILE_SIZE);
        }

        return false;
    }

    public delegate void OnArriveAtTile(MapTile mapTile, MapUnit mapUnit);
    public OnArriveAtTile onArriveAtTile;
    public UnityEvent onArriveAtTileUnity;

    public TileType tileType;
    public bool isValid;

    public WallStyle wallStyle;

    public List<MapTile> GetNeighbors(bool includeWalls)
	{
		IEnumerable<MapTile> neighbors = (Enum.GetValues(typeof(Direction)) as Direction[]).Select(dir => GetNeighbor(dir));
		if (!includeWalls)
		{
			neighbors = neighbors
				.Where(tile =>
					tile != null
					&& tile.tileType != TileType.Wall
				);
		}
		return neighbors.ToList();
	}

    public MapTile GetNeighbor(Vector3 offset)
    {
        if (offset == Vector3.up)
        {
            return GetNeighbor(Direction.Up);
        }
        if (offset == Vector3.down)
        {
            return GetNeighbor(Direction.Down);
        }
        if (offset == Vector3.left)
        {
            return GetNeighbor(Direction.Left);
        }
        if (offset == Vector3.right)
        {
            return GetNeighbor(Direction.Right);
        }

        return null;
    }

    public MapTile GetNeighbor(Direction direction)
    {
		return Map.Instance.GetMapTile(Coordinates + direction.ToVector2Int());
    }

	private void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;

		int modX = Mathf.RoundToInt(transform.position.x / 32);
		int modY = Mathf.RoundToInt(transform.position.y / 32);
		transform.position = new Vector3(modX * 32, modY * 32);
#endif
	}
}
