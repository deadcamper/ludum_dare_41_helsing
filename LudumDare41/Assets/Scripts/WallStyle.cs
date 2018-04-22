using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class WallStyle : ScriptableObject
{
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
	public Sprite floor;
	public Sprite fill;

	public Sprite wall;
	public Sprite leftPiece;
	public Sprite rightPiece;


	public Sprite LockedDoorFrame;
	public Sprite doorFrame;
	public Sprite closedDoor;
	public Sprite openDoor;

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

	public List<SpriteInfo> GetWallSprites(Map map, Vector2Int coordinates)
	{
		List<SpriteInfo> spriteInfos = new List<SpriteInfo>();

		MapTile center = map.GetMapTile(coordinates);

		if (center.tileType == TileType.Floor)
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

				MapTile n = microMap[1, 2];
				MapTile ne = microMap[2, 2];
				MapTile e = microMap[2, 1]; ;
				MapTile se = microMap[2, 0]; ;
				MapTile s = microMap[1, 0];
				MapTile sw = microMap[0, 0];
				MapTile w = microMap[0, 1];
				MapTile nw = microMap[0, 2];

				if (n != null)
				{
					if(n.tileType == TileType.Wall)
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), wall, sortingLayer, 3));
					}
					else if (n.tileType == TileType.Door)
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), wall, sortingLayer, 3));
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), closedDoor, sortingLayer, 4));
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), LockedDoorFrame, sortingLayer, 5));
					}
				}
				//if (n == null || n.tileType == TileType.Wall || ne == null || ne.tileType == TileType.Wall)
				{
					if ((e != null && e.tileType != TileType.Wall && e.tileType != TileType.Door))
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), rightPiece, sortingLayer, 3));
					}
				}
				//if (n == null || n.tileType == TileType.Wall || nw == null || nw.tileType == TileType.Wall)
				{
					if ((w != null && w.tileType != TileType.Wall && w.tileType != TileType.Door))
					{
						spriteInfos.Add(new SpriteInfo(Quaternion.Euler(0, 0, rotations[dir]), leftPiece, sortingLayer, 3));
					}
				}
			}
		}
		if (spriteInfos.Count > 0)
		{
			Debug.Log("sfdgsfdgsdfgsdgf");
		}
		return spriteInfos;

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