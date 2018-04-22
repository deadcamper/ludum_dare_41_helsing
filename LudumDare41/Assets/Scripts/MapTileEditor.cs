using UnityEngine;

[RequireComponent(typeof(MapTile))]
[ExecuteInEditMode]
public class MapTileEditor : MonoBehaviour
{
	MapTile _mapTile;
	MapTile MapTile
	{
		get
		{
			if (_mapTile == null)
			{
				_mapTile = GetComponent<MapTile>();
			}
			return _mapTile;
		}
	}


	private void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) 
            return;

		MapTile.RegenerateDecorations(Map.Instance);

		int modX = Mathf.RoundToInt(transform.position.x / 32);
        int modY = Mathf.RoundToInt(transform.position.y / 32);
        transform.position = new Vector3(modX * 32, modY * 32);
#endif
    }
}
