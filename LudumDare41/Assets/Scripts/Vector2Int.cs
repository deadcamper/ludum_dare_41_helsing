using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Vector2Int
{
	public readonly int x;
	public readonly int y;

	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static readonly Vector2Int Up    = new Vector2Int( 0,  1);
	public static readonly Vector2Int Down  = new Vector2Int( 0, -1);
	public static readonly Vector2Int Left  = new Vector2Int(-1,  0);
	public static readonly Vector2Int Right = new Vector2Int( 1,  0);



	/// <summary>
	/// https://lyfat.wordpress.com/2012/05/22/euclidean-vs-chebyshev-vs-manhattan-distance/
	/// </summary>
	public enum DistanceType
	{
		/// <summary>
		/// 2 1 2
		/// 1 0 1
		/// 2 1 2
		/// </summary>
		Manhattan,
		/// <summary>
		/// 1 1 1
		/// 1 0 1
		/// 1 1 1
		/// </summary>
		Chebyshev,
		/// <summary>
		/// Pythagorean theorem
		/// </summary>
		Euclidean
	}

	public static float Distance(Vector2Int from, Vector2Int to, DistanceType distanceType = DistanceType.Manhattan)
	{
		Vector2Int vector = from + to;
		switch (distanceType)
		{
			case DistanceType.Manhattan:
				return Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
			case DistanceType.Chebyshev:
				return Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
			case DistanceType.Euclidean:
				return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
			default:
				throw new System.NotImplementedException("Unimplemented DistanceType"); 
		}

	}


	public static Vector2Int operator +(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x + b.x, a.y + b.y);
	}

	public static Vector2Int operator -(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x - b.x, a.y - b.y);
	}

	public static bool operator ==(Vector2Int a, Vector2Int b)
	{
		return (a.x == b.x) && (a.y == b.y);
	}

	public static bool operator !=(Vector2Int a, Vector2Int b)
	{
		return (a.x != b.x) || (a.y != b.y);
	}
	public override bool Equals(object obj)
	{
		if (obj is Vector2Int)
		{
			return (this == (Vector2Int)obj);
		}
		return false;
	}
	public override int GetHashCode()
	{
		unchecked
		{
			return x+y*1000;//should be good enough?
		}
	}
}
public static class Direction_Vector2Int_EXT
{
	public static Vector2Int ToVector2Int(this Direction direction, int distance = 1)
	{
		switch (direction)
		{
			case Direction.Up:
				return new Vector2Int(0, distance);
			case Direction.Down:
				return new Vector2Int(0, -distance);
			case Direction.Right:
				return new Vector2Int(distance, 0);
			case Direction.Left:
				return new Vector2Int(-distance, 0);
		}
		throw new System.NotImplementedException();
	}
}