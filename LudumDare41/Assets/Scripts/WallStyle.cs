using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class WallStyle : ScriptableObject
{
	/*
	private static WallStyle _instance;
	public static WallStyle Instance{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<WallStyle>("WallStyle");
			}
			return _instance;
		}
	}
    */

	public Sprite floor;
	public Sprite fill;

	public Sprite wall;
	public Sprite leftPiece;
	public Sprite rightPiece;


	public Sprite LockedDoorFrame;
	public Sprite doorFrame;
	public Sprite closedDoor;
	public Sprite openDoor;

	public GameObject floorPrefab;
	public GameObject fillPrefab;

	public GameObject wallPrefab;
	public GameObject leftPiecePrefab;
	public GameObject rightPiecePrefab;


	public GameObject LockedDoorFramePrefab;
	public GameObject doorFramePrefab;
	public GameObject closedDoorPrefab;
	public GameObject openDoorPrefab;

	public class SpriteInfo
	{
		public readonly Quaternion rotation;
		public readonly Sprite sprite;
		public readonly SortingLayer sortingLayer;
		public readonly int orderInLayer;
		public SpriteInfo(Quaternion rotation, Sprite sprite, SortingLayer sortingLayer, int orderInLayer)
		{
			this.rotation = rotation;
			this.sprite = sprite;
			this.sortingLayer = sortingLayer;
			this.orderInLayer = orderInLayer;
		}
	}

	public class PrefabInfo
	{
		public readonly Quaternion rotation;
		public readonly GameObject prefab;
		public readonly SortingLayer sortingLayer;
		public readonly int orderInLayer;
		public PrefabInfo(Quaternion rotation, GameObject prefab, SortingLayer sortingLayer, int orderInLayer)
		{
			this.rotation = rotation;
			this.prefab = prefab;
			this.sortingLayer = sortingLayer;
			this.orderInLayer = orderInLayer;
		}
	}

	public List<PrefabInfo> GetMeshes(Map map, Vector2Int coordinates)
	{
		List<PrefabInfo> prefabInfos = new List<PrefabInfo>();

		prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, 0), floorPrefab, SortingLayer.layers.First(l => l.name == "Floors"), 2));


		MapTile center = map.GetMapTile(coordinates);

		if (TileMatchesTileType(center, TileType.Floor, TileType.Exit))
		{

			int[] rotations = new int[] { 0, 90, 180, 270 };

			MapTile[,] microMap;
			{
				MapTile n = map.GetMapTile(coordinates + Vector2Int.Up);
				MapTile ne = map.GetMapTile(coordinates + Vector2Int.Up + Vector2Int.Right);
				MapTile nw = map.GetMapTile(coordinates + Vector2Int.Up + Vector2Int.Left);

				MapTile s = map.GetMapTile(coordinates + Vector2Int.Down);
				MapTile se = map.GetMapTile(coordinates + Vector2Int.Down + Vector2Int.Right);
				MapTile sw = map.GetMapTile(coordinates + Vector2Int.Down + Vector2Int.Left);

				MapTile w = map.GetMapTile(coordinates + Vector2Int.Left);
				MapTile e = map.GetMapTile(coordinates + Vector2Int.Right);

				microMap = new MapTile[,] { { nw, n, ne }, { w, center, e }, { se, s, sw }, { sw, w, nw } };
			}

			SortingLayer sortingLayer = SortingLayer.layers.First(l => l.name == "Walls");


			for (int dir = 0; dir < 4; dir++)
			{
				microMap = RotateMatrix<MapTile>(microMap, 3);

				//MapTile nw = microMap[0, 2];
				MapTile n = microMap[1, 2];
				//MapTile ne = microMap[2, 2];
				MapTile w = microMap[0, 1];
				/// center MapTile c = microMap[1, 1];
				MapTile e = microMap[2, 1];
				//MapTile sw = microMap[0, 0];
				//MapTile s = microMap[1, 0];
				//MapTile se = microMap[2, 0];

				if (n != null)
				{
					if (n.tileType == TileType.Wall)
					{
						prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), wallPrefab, sortingLayer, 3));
					}
					else if (n.tileType == TileType.Door || n.tileType == TileType.EntryDoor)
					{
						prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), wallPrefab, sortingLayer, 3));

						if (!n.isValid)
							prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), closedDoorPrefab, sortingLayer, 4));
						else
							prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), openDoorPrefab, sortingLayer, 4));

						if (n.tileType == TileType.Door)
						{
							prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), LockedDoorFramePrefab, sortingLayer, 5));
						}
						else if (n.tileType == TileType.EntryDoor)
						{
							prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), doorFramePrefab, sortingLayer, 5));
						}
					}
				}
				//if (n == null || n.tileType == TileType.Wall || ne == null || ne.tileType == TileType.Wall)
				{
					if (TileMatchesTileType(e, TileType.Floor, TileType.Exit))
					{
						prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), rightPiecePrefab, sortingLayer, 3));
					}
				}
				//if (n == null || n.tileType == TileType.Wall || nw == null || nw.tileType == TileType.Wall)
				{
					if (TileMatchesTileType(w, TileType.Floor, TileType.Exit))
					{
						prefabInfos.Add(new PrefabInfo(Quaternion.Euler(0, 0, rotations[dir]), leftPiecePrefab, sortingLayer, 3));
					}
				}
			}
		}
		return prefabInfos;
	}

	public List<SpriteInfo> GetWallSprites(Map map, Vector2Int coordinates)
	{
		List<SpriteInfo> spriteInfos = new List<SpriteInfo>();

		MapTile center = map.GetMapTile(coordinates);

		if (TileMatchesTileType(center, TileType.Floor, TileType.Exit))
		{
			int[] rotations = new int[] { 0, 90, 180, 270 };

			MapTile[,] microMap;
			{
				MapTile n = map.GetMapTile(coordinates + Vector2Int.Up);
				MapTile ne = map.GetMapTile(coordinates + Vector2Int.Up + Vector2Int.Right);
				MapTile nw = map.GetMapTile(coordinates + Vector2Int.Up + Vector2Int.Left);

				MapTile s = map.GetMapTile(coordinates + Vector2Int.Down);
				MapTile se = map.GetMapTile(coordinates + Vector2Int.Down + Vector2Int.Right);
				MapTile sw = map.GetMapTile(coordinates + Vector2Int.Down + Vector2Int.Left);

				MapTile w = map.GetMapTile(coordinates + Vector2Int.Left);
				MapTile e = map.GetMapTile(coordinates + Vector2Int.Right);

				microMap = new MapTile[,] { { nw, n, ne }, { w, center, e }, { se, s, sw }, { sw, w, nw } };
			}

			SortingLayer sortingLayer = SortingLayer.layers.First(l => l.name == "Walls");

			for (int dir = 0; dir < 4; dir++)
			{
				microMap = RotateMatrix<MapTile>(microMap, 3);
                
                //MapTile nw = microMap[0, 2];
                MapTile n = microMap[1, 2];
                //MapTile ne = microMap[2, 2];
                MapTile w = microMap[0, 1];
                /// center MapTile c = microMap[1, 1];
                MapTile e = microMap[2, 1];
                //MapTile sw = microMap[0, 0];
                //MapTile s = microMap[1, 0];
                //MapTile se = microMap[2, 0];
                
                if (n != null)
				{
					if(n.tileType == TileType.Wall)
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), wall, sortingLayer, 3));
					}
					else if (n.tileType == TileType.Door || n.tileType == TileType.EntryDoor)
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), wall, sortingLayer, 3));

                        if (!n.isValid)
						    spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), closedDoor, sortingLayer, 4));
                        else
                            spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), openDoor, sortingLayer, 4));

						if (n.tileType == TileType.Door)
						{
							spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), LockedDoorFrame, sortingLayer, 5));
						}
						else if (n.tileType == TileType.EntryDoor)
						{
							spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), doorFrame, sortingLayer, 5));
						}
					}
				}
                //if (n == null || n.tileType == TileType.Wall || ne == null || ne.tileType == TileType.Wall)
                {
                    if (TileMatchesTileType(e, TileType.Floor, TileType.Exit))
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), rightPiece, sortingLayer, 3));
					}
				}
				//if (n == null || n.tileType == TileType.Wall || nw == null || nw.tileType == TileType.Wall)
				{
					if (TileMatchesTileType(w, TileType.Floor, TileType.Exit))
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), leftPiece, sortingLayer, 3));
					}
				}
			}
		}
		return spriteInfos;

	}

    private bool TileMatchesTileType(MapTile tile, params TileType?[] types)
    {
        if (tile == null)
            return types.Contains(null);

        return types.Contains(tile.tileType);
    }

	static T[,] RotateMatrix<T>(T[,] matrix, int n) {
		T[,] ret = new T[n, n];

		for (int i = 0; i < n; ++i)
		{
			for (int j = 0; j < n; ++j)
			{
				ret[i, j] = matrix[n - j - 1, i];
			}
		}

		return ret;
	}
}