using System.Collections.Generic;
using UnityEngine;

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
		if(tileType == TileType.Wall || tileType == TileType.Door) SpriteRenderer.sprite = WallStyle.Instance.fill;
		else SpriteRenderer.sprite = WallStyle.Instance.floor;

		if (!Application.isPlaying)
			DestroyImmediate(GeneratedDecorationsParent.gameObject);
		else
			foreach (Transform child in GeneratedDecorationsParent)
			{
				Destroy(child.gameObject);
			}

				if (WallStyle.Instance != null)
		{
			var spriteInfos = WallStyle.Instance.GetWallSprites(map, Coordinates);
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

    public TileType tileType;
    public bool isValid;
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

	private void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;

		RegenerateDecorations(Map.Instance);

		int modX = Mathf.RoundToInt(transform.position.x / 32);
		int modY = Mathf.RoundToInt(transform.position.y / 32);
		transform.position = new Vector3(modX * 32, modY * 32);
#endif
	}
}
