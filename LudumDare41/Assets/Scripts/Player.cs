using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : TurnTaker, Killable
{
	public GameObject liveSprite;
	public GameObject deadSprite;
	public GameObject gunArmSprite;
	public GameObject stakeArmSprite;

    private int lastFrameCount = 0;

    public Game game;

    public bool forceCamera = true;

    public Direction direction;

    [System.Serializable]
    public struct AudioLibraryEntry
    {
        public string name;
        public AudioSource audioSource;
    }
    public List<AudioLibraryEntry> audioLibrary;

    public MapUnit MapUnit { get; private set; }

    private bool turnComplete = false;
    public Inventory Inventory { get; private set; }

	private bool _dead;
	private bool Dead
	{
		get { return _dead; }
		set
		{
			_dead = value;
			liveSprite.SetActive(!value);
			deadSprite.SetActive(value);
		}
	}

    private void Awake()
    {
        MapUnit = new MapUnit(transform.position, this);
        Inventory = new Inventory();
        Inventory.Items.Add(ItemType.Stake, 1);
        Inventory.Items.Add(ItemType.SilverBullet, 6);
        Inventory.Items.Add(ItemType.Key, 0);
		Dead = false;
		Inventory.onItemChange += (a, b) =>
		{
			stakeArmSprite.SetActive(Inventory.Items.Any(kvp => kvp.Key == ItemType.Stake        && kvp.Value > 0));
			gunArmSprite.SetActive(  Inventory.Items.Any(kvp => kvp.Key == ItemType.SilverBullet && kvp.Value > 0));
		};

        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
    }

    public void PlayClip(string name)
    {
        foreach (AudioLibraryEntry entry in audioLibrary)
        {
            if (entry.name.Equals(name))
            {
                if (!entry.audioSource.isPlaying)
                    entry.audioSource.Play();
                break;
            }
        }
    }

    private void Update()
    {
        if (forceCamera)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + (Vector3.back * 20), 0.3f);
        }

        if (MapUnit.CurrentTile != null)
        {
            transform.position = Vector3.Lerp(transform.position, MapUnit.CurrentTile.transform.position, 0.3f);
        }

        transform.rotation = DirectionUtil.GetSpriteRotationForDirection(direction);
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

            if (lastFrameCount != Time.frameCount)
            {
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

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    if (AttemptFireGun())
                        turnComplete = true;
                }
            }
            lastFrameCount = Time.frameCount;

            if (nextNode != null)
            {
                if (nextNode.isValid)
                {
                    MapUnit.CurrentTile = nextNode;
                    turnComplete = true;
                    PlayClip("walk");
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
                        GameUI.DisplayMessage("You don't have a key.");
                    }
                }
            }

            yield return null;
        }
    }

    private bool AttemptFireGun()
    {
        return false;
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
                    PlayClip("door");
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
        Dead = true;
    }

    public bool isDead()
    {
        return Dead;
    }
}
