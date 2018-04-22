using UnityEngine;

public class Coffin : MonoBehaviour
{
    public MapUnit MapUnit { get; private set; }

    public GameObject enemyTemplateToSpawn;

    public SpriteRenderer render;

    public Sprite coffinOpen;
    public Sprite coffinClosed;

    public AudioSource coffinSound;

    private void Start()
    {
        MapUnit = new MapUnit(transform.position, null);
        render.sprite = coffinClosed;
    }
        
    public void OpenCoffin()
    {
        render.sprite = coffinOpen;
        //coffinSound.Play();

        //Spawn baddie
        if (enemyTemplateToSpawn != null)
        {
            GameObject enemy = Instantiate(enemyTemplateToSpawn, MapUnit.CurrentTile.transform.position, Quaternion.identity);
            TurnTaker turnTaker = enemy.GetComponent<TurnTaker>();

            if (turnTaker != null)
            {
                TurnManager turns = FindObjectOfType<TurnManager>();
                turns.AddTurnTaker(enemy.GetComponent<TurnTaker>());
            }
        }
    }
}