using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : TurnTaker, Killable
{
    public Game game;
    public Direction direction;

    [System.Serializable]
    public struct AudioLibraryEntry
    {
        public string name;
        public AudioClip clip;
    }
    public List<AudioLibraryEntry> audioLibrary;
    public AudioSource audioSource;

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
        Inventory.onItemChange += OnItemAdded;
    }

    public void PlayClip(string name)
    {
        foreach (AudioLibraryEntry entry in audioLibrary)
        {
            if (entry.name.Equals(name))
            {
                audioSource.clip = entry.clip;
                break;
            }
        }

        audioSource.Play();
    }

    private void OnItemAdded(ItemType itemType, int qty)
    {
        // pickup an item
        if (qty > 0)
        {
            PlayClip("collect");
        }
    }

    private void Update()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + (Vector3.back * 20), 0.1f);

        if (MapUnit.CurrentTile != null)
        {
            transform.position = Vector3.Lerp(transform.position, MapUnit.CurrentTile.transform.position, 0.3f);
        }

        RotateSpriteToDirection();
    }

    private void RotateSpriteToDirection()
    {
        switch(direction)
        {
            case Direction.Up:
                transform.rotation = Quaternion.AngleAxis(180, Vector3.right);
                break;

            case Direction.Down:
                transform.rotation = Quaternion.AngleAxis(0, Vector3.right);
                break;

            case Direction.Left:
                transform.rotation = Quaternion.AngleAxis(270, Vector3.forward);
                break;

            case Direction.Right:
                transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
                break;
        }
    }

    public override IEnumerator TurnLogic()
    {
        MapTile nextNode = null;
        turnComplete = false;

        while (!turnComplete)
        {
            if (isDead() || game.IsPaused())
            {
                yield return null;
                continue; //Hack to prevent turn completion
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                // up
                if (direction == Direction.Up)
                    nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.up);
                else
                {
                    direction = Direction.Up;
                    turnComplete = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                // down
                if (direction == Direction.Down)
                    nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.down);
                else
                {
                    direction = Direction.Down;
                    turnComplete = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                // left
                if (direction == Direction.Left)
                    nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.left);
                else
                {
                    direction = Direction.Left;
                    turnComplete = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                // right
                if (direction == Direction.Right)
                    nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.right);
                else
                {
                    direction = Direction.Right;
                    turnComplete = true;
                }
            }

            if (nextNode != null)
            {
                if (nextNode.isValid)
                {
                    MapUnit.CurrentTile = nextNode;
                    turnComplete = true;
                }
                else
                {
                    if (AttemptUseKey(nextNode))
                    {
                        // used a key! (door is now marked as valid)
                        MapUnit.CurrentTile = nextNode;
                        turnComplete = true;
                    }
                    else
                    {
                        nextNode = null;
                    }
                }
            }

            yield return null;
        }
    }

    private bool AttemptUseKey(MapTile nextNode)
    {
        if (nextNode == null)
            return false;

        if (nextNode.isValid == true)
            return false;

        switch(nextNode.tileType)
        {
            case TileType.Door: // TODO as we add more "door" types / "key" types, we gotta account for that here d
                if (Inventory.RemoveItem(ItemType.Key, 1))
                {
                    nextNode.isValid = true;
                    return true;
                }
                break;
        }

        return false;
    }

    public override bool TurnComplete()
    {
        return turnComplete;
    }

    public void Die()
    {
        PlayClip("death");
        dead = true;
    }

    public bool isDead()
    {
        return dead;
    }
}
