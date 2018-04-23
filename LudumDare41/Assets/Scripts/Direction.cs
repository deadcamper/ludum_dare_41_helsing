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

    public static Direction GetDirectionBetween(Vector3 pos1, Vector3 pos2)
    {
        int diffX = Mathf.RoundToInt(pos1.x - pos2.x);
        int diffY = Mathf.RoundToInt(pos1.y - pos2.y);

        if (diffX < 0)
        {
            return Direction.Right;
        }
        else if (diffX > 0)
        {
            return Direction.Left;
        }

        if (diffY < 0)
        {
            return Direction.Up;
        }
        else if (diffY > 0)
        {
            return Direction.Down;
        }

        return Direction.Up;
    }
}