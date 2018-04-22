using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionUtil
{
	public static IEnumerable<Direction> All
	{
		get
		{
			yield return Direction.Up;
			yield return Direction.Down;
			yield return Direction.Left;
			yield return Direction.Right;
		}
	}
	public static Quaternion GetSpriteRotationForDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Quaternion.AngleAxis(180, Vector3.right);

            case Direction.Down:
                return Quaternion.AngleAxis(0, Vector3.right);

            case Direction.Left:
                return Quaternion.AngleAxis(270, Vector3.forward);

            case Direction.Right:
                return Quaternion.AngleAxis(90, Vector3.forward);
        }

        return Quaternion.AngleAxis(180, Vector3.right);
    }
}