using System.Collections;
using UnityEngine;

public class Player : TurnTaker, Killable
{
    public MapUnit MapUnit { get; private set; }

    private bool turnComplete = false;
    public Inventory Inventory { get; private set; }

    private bool dead = false;

    private void Start()
    {
        MapUnit = new MapUnit(transform.position, this);
        Inventory = new Inventory();
        Inventory.Items.Add(ItemType.Stake, 1);
        Inventory.Items.Add(ItemType.SilverBullet, 6);
        Inventory.Items.Add(ItemType.Key, 0);
    }

    private void Update()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + (Vector3.back * 20), 0.1f);

        if (MapUnit.CurrentTile != null)
        {
            transform.position = Vector3.Lerp(transform.position, MapUnit.CurrentTile.transform.position, 0.3f);
        }
    }

    public override IEnumerator TurnLogic()
    {
        MapTile nextNode = null;
        turnComplete = false;
        while (nextNode == null)
        {
            if (isDead())
            {
                yield return null;
                continue; //Hack to prevent turn completion
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                // up
                nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.up);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                // down
                nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.down);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                // left
                nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.left);
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                // right
                nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.right);
            }

            // TEMP
            if (Input.GetKeyUp(KeyCode.Space))
                Inventory.RemoveItem(ItemType.Key, 1);

            if (nextNode != null)
            {
                MapUnit.CurrentTile = nextNode;
            }

            yield return null;
        }
        turnComplete = true;
    }

    public override bool TurnComplete()
    {
        return turnComplete;
    }

    public void Die()
    {
        dead = true;
    }

    public bool isDead()
    {
        return dead;
    }
}
