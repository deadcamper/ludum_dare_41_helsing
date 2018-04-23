using UnityEngine;

public class CustomLevel3 : MonoBehaviour
{
    public GameObject[] AmbushSpawnPoints;
    public Enemy enemySpawnTemplate;

    public void SpawnAmbush()
    {
        TurnManager turns = FindObjectOfType<TurnManager>();

        foreach (GameObject spawn in AmbushSpawnPoints)
        {
            Enemy enemy = Instantiate(enemySpawnTemplate, spawn.transform.position, Quaternion.identity);
            TurnTaker turnTaker = enemy.GetComponent<TurnTaker>();

            if (turnTaker != null)
            {
                turns.AddTurnTaker(enemy.GetComponent<TurnTaker>());
            }
        }
    }
}
