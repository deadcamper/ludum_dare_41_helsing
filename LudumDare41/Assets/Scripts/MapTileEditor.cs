using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapTile))]
public class MapTileEditor : Editor
{
    private void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) 
            return;

        MapTile tile = (MapTile)target;
        int modX = Mathf.RoundToInt(tile.transform.position.x / 32);
        int modY = Mathf.RoundToInt(tile.transform.position.y / 32);
        tile.transform.position = new Vector3(modX * 32, modY * 32);
#endif
    }

}
