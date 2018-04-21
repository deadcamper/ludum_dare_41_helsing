using System.Collections;
using UnityEngine;

public class Player : TurnTaker
{
    public MapUnit MapUnit { get; private set; }
    private bool turnComplete = false;

    private void Start()
    {
        MapUnit = new MapUnit(transform.position);
    }

    private void Update()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + (Vector3.back * 20), 0.1f);
    }

    public override IEnumerator TurnLogic()
    {
        MapTile nextNode = null;
        turnComplete = false;
        while (nextNode == null)
        {
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

            if (nextNode != null)
            {
                MapUnit.CurrentTile = nextNode;
                transform.position = MapUnit.CurrentTile.transform.position;
            }

            yield return null;
        }
        turnComplete = true;
    }

    public override bool TurnComplete()
    {
        return turnComplete;
    }
}
