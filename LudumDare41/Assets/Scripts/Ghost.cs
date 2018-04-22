using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Ghost : MonoBehaviour
{
	public GameObject VisibleSprite;
	public GameObject InvisibleSprite;

	public void Update()
	{
		Vector2 relativePosition = transform.position - Player.Instance.transform.position;
		bool visible = true;
		switch (Player.Instance.direction)
		{
			case Direction.Up:
				visible = (relativePosition.y <= 1);
				break;
			case Direction.Down:
				visible = (relativePosition.y >= -1);
				break;
			case Direction.Right:
				visible = (relativePosition.x <= 1);
				break;
			case Direction.Left:
				visible = (relativePosition.x >= -1);
				break;
		}
		VisibleSprite.SetActive(visible);
		InvisibleSprite.SetActive(!visible);
	}
}
