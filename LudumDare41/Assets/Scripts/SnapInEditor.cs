using UnityEngine;

[ExecuteInEditMode]
public class SnapInEditor : MonoBehaviour
{
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
