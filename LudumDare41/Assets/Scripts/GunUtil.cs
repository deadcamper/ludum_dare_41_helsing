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

        while (nextMapTile != null && nextMapTile.tileType == TileType.Floor)
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

    public static void KillEnemiesInSight(Player player)
    {
        List<Enemy> enemies = GetEnemiesInSight(player);
        foreach (Enemy enemy in enemies)
        {
            enemy.Die();
        }
    }
}