using UnityEngine;

public class HideSpriteOnStart : MonoBehaviour
{
	void Start()
	{
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.enabled = false;
	}
}