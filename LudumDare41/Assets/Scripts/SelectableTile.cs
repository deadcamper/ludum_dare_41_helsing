using UnityEngine;

public class SelectableTile : MonoBehaviour
{
    Player player;
    MapTile tile;

    private void OnMouseDown()
    {
        // selected!
        player.ProcessTileClick(tile);
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        tile = GetComponentInParent<MapTile>();
    }

    private void Update()
    {
        if (player.MapUnit.CurrentTile == tile)
        {
            Destroy(gameObject);
        }
    }
}
