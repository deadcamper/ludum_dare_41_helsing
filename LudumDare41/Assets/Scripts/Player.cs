using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : TurnTaker, Killable
{
	static Player _instance;
	public static Player Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<Player>();
			}
			return _instance;
		}
	}

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
        Dead = false;
        Inventory.GetInstance().onItemChange += UpdatePlayerSpritesWithInventory;
        UpdatePlayerSpritesWithInventory(ItemType.Key, 0, 0); //Force trigger to update player, values don't matter

        if (game == null)
        {
            game = FindObjectOfType<Game>();
        }
    }

    private void UpdatePlayerSpritesWithInventory(ItemType t, int q, int tot)
    {
        if (stakeArmSprite)
            stakeArmSprite.SetActive(Inventory.GetInstance().HasItem(ItemType.MetalStake) || Inventory.GetInstance().HasItem(ItemType.Stake));

        if (gunArmSprite)
            gunArmSprite.SetActive(Inventory.GetInstance().Items.Any(kvp => kvp.Key == ItemType.SilverBullet && kvp.Value > 0));
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
					turnComplete = ProcessDirectionalInput(Direction.Up);

				if (Input.GetKeyDown(KeyCode.S))
					turnComplete = ProcessDirectionalInput(Direction.Down);

				if (Input.GetKeyDown(KeyCode.A))
					turnComplete = ProcessDirectionalInput(Direction.Left);

				if (Input.GetKeyDown(KeyCode.D))
					turnComplete = ProcessDirectionalInput(Direction.Right);

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
			
            yield return null;
        }
    }

	bool ProcessDirectionalInput(Direction inputDirection)
	{
		if (direction == inputDirection)
			return TryMoveToTile(MapUnit.CurrentTile.GetNeighbor(direction));
		else
		{
			direction = inputDirection;
			return false;
		}
	}

	bool TryMoveToTile(MapTile tile)
	{
		bool complete = false;
		if (tile != null)
		{
			if (tile.isValid)
			{
				MapUnit.CurrentTile = tile;
                complete = true;
				PlayClip("walk");
			}
			else if(tile.tileType == TileType.Door)
			{
				if (AttemptUseKey(tile))
				{
					// used a key! (door is now marked as valid)
					MapUnit.CurrentTile = tile;
                    complete = true;
				}
				else
				{
					PlayClip("locked");
					GameUI.DisplayMessage("You don't have a key.");
				}
			}
			else if (tile.tileType == TileType.EntryDoor)
			{
				GameUI.DisplayMessage("This is too important. You can't go back.");
			}
		}
        return complete;
	}


    private bool AttemptFireGun()
    {
        if (Inventory.GetInstance().RemoveItem(ItemType.SilverBullet, 1))
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
                if (Inventory.GetInstance().RemoveItem(ItemType.Key, 1))
                {
                    PlayClip("door");
                    nextNode.isValid = true;
                    
                    nextNode.RegenerateDecorations(Map.Instance);
                    foreach (MapTile neighbor in nextNode.GetNeighbors(false))
                    {
                        neighbor.RegenerateDecorations(Map.Instance);
                    }

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
