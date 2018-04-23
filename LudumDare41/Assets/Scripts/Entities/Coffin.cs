using UnityEngine;

public class Coffin : MonoBehaviour
{
    public MapUnit MapUnit { get; private set; }

    public GameObject enemyTemplateToSpawn;

    public SpriteRenderer render;

    public Sprite coffinOpen;
    public Sprite coffinClosed;

    public AudioSource coffinSound;

    public bool startOpen;
    private bool isOpen;

    private void Start()
    {
        MapUnit = new MapUnit(transform.position, null);
        render.sprite = coffinClosed;

        if(startOpen)
        {
            render.sprite = coffinOpen;
            isOpen = true;
        }
    }
        
    public void OpenCoffin()
    {
        if (isOpen)
            return;

        render.sprite = coffinOpen;
        coffinSound.Play();

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

        isOpen = true;
    }

    public void OpenAllCoffins()
    {
        //Used for the final boss
        Coffin[] coffins = FindObjectsOfType<Coffin>();
        foreach (Coffin c in coffins)
        {
            if (c != null && !c.isOpen)
            {
                c.OpenCoffin();
            }
        }
    }
}