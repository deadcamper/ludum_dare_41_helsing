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
    public MuzzleFlash muzzleFlash;

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
        Inventory.Items.Add(ItemType.SilverBullet, 0);
        Inventory.Items.Add(ItemType.Key, 0);
        Inventory.Items.Add(ItemType.MetalStake, 0);
        Dead = false;
        Inventory.onItemChange += UpdatePlayerSpritesWithInventory;
        UpdatePlayerSpritesWithInventory(ItemType.Key, 0); //Force trigger to update player

        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
    }

    private void UpdatePlayerSpritesWithInventory(ItemType t, int q)
    {
        stakeArmSprite.SetActive(Inventory.Items.Any(kvp => kvp.Key == ItemType.Stake && kvp.Value > 0));
        gunArmSprite.SetActive(Inventory.Items.Any(kvp => kvp.Key == ItemType.SilverBullet && kvp.Value > 0));
    }

    public void PlayClip(string name, bool isPlayingOverride = false)
    {
        foreach (AudioLibraryEntry entry in audioLibrary)
        {
            if (entry.name.Equals(name))
            {
                if (!entry.audioSource.isPlaying || isPlayingOverride)
                {
                    // pitch shift for variations
                    if (name.Equals("walk"))
                    {
                        entry.audioSource.pitch = 1.0f + Random.value;
                    }
                    if (name.Equals("gun"))
                    {
                        entry.audioSource.pitch = 1.0f - (Random.value * 0.2f);
                    }

                    entry.audioSource.Play();
                }
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

        HUD.Show();

        while (!turnComplete)
        {
            if (isDead() || game.IsPaused())
            {
                yield return null;
                continue; //Hack to prevent turn completion
            }

            if (lastFrameCount != Time.frameCount)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    // up
                    if (direction == Direction.Up)
                        nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.up);
                    else
                    {
                        direction = Direction.Up;
                        //turnComplete = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    // down
                    if (direction == Direction.Down)
                        nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.down);
                    else
                    {
                        direction = Direction.Down;
                        //turnComplete = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    // left
                    if (direction == Direction.Left)
                        nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.left);
                    else
                    {
                        direction = Direction.Left;
                        //turnComplete = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    // right
                    if (direction == Direction.Right)
                        nextNode = MapUnit.CurrentTile.GetNeighbor(Vector3.right);
                    else
                    {
                        direction = Direction.Right;
                        //turnComplete = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (AttemptFireGun())
                    {
                        GunUtil.KillEnemiesInSight(this);
                        turnComplete = true;
                    }
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
                        PlayClip("locked");
                        GameUI.DisplayMessage("You don't have a key.");
                    }
                }
            }

            yield return null;
        }
    }

    private bool AttemptFireGun()
    {
        if (Inventory.RemoveItem(ItemType.SilverBullet, 1))
        {
            muzzleFlash.Fire();
            PlayClip("gun", true);
            return true;
        }

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
