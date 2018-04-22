using System.Collections.Generic;
using UnityEngine;

public static class GunUtil
{
    public static List<Enemy> GetEnemiesInSight(Player player)
    {
        List<Enemy> enemies = new List<Enemy>();

        // get potential enemies
        Enemy[] potentialEnemies = GameObject.FindObjectsOfType<Enemy>();

        // start at our player's current position
        MapTile currentMapTile = player.MapUnit.CurrentTile;
        MapTile nextMapTile = currentMapTile.GetNeighbor(player.direction);

        while (nextMapTile != null && !IsObstacle(nextMapTile))
        {
            // current tile update
            currentMapTile = nextMapTile;

            // next tile update
            nextMapTile = currentMapTile.GetNeighbor(player.direction);

            // is there an enemy at this current tile?
            foreach (Enemy potentialEnemy in potentialEnemies)
            {
                if (potentialEnemy.MapUnit.CurrentTile == currentMapTile)
                {
                    enemies.Add(potentialEnemy);
                }
            }
        }

        return enemies;
    }

    private static bool IsObstacle(MapTile mapTile)
    {
        switch (mapTile.tileType)
        {
            case TileType.Wall:
                return true;

            case TileType.Door:
                return !mapTile.isValid; // you can shoot through open doors only
        }

        return false;
    }

    public static void KillEnemiesInSight(Player player)
    {
        List<Enemy> enemies = GetEnemiesInSight(player);
        foreach (Enemy enemy in enemies)
        {
            enemy.Die();
        }
    }
}