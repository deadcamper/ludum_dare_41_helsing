using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TurnTaker, Killable
{
    private int backtrackHistorySize = 4;
    public int turnCountdown;
    public int tileCount = 1;
    private int currentCountdown;
    public MapUnit MapUnit { get; private set; }
    private List<MapTile> recentlyVisited = new List<MapTile>();

    public AudioSource deathSound;

	public GameObject liveSprite;
	public GameObject deadSprite;

	private Direction direction;
    private Player player;

	private bool _dead = false;
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

    void Awake()
    {
        player = FindObjectOfType<Player>();
        MapUnit = new MapUnit(transform.position, this);
    }

    // Use this for initialization
    void Start()
    {
        currentCountdown = turnCountdown;
		Dead = false;
	}

    private void Update()
    {
        if (MapUnit.CurrentTile != null)
        {
            transform.position = Vector3.Lerp(transform.position, MapUnit.CurrentTile.transform.position, 0.2f);
        }

        transform.rotation = Quaternion.Euler(0,0, Mathf.LerpAngle(transform.rotation.eulerAngles.z, DirectionUtil.GetSpriteRotationForDirection(direction).eulerAngles.z, 0.2f));
    }

    public override bool TurnComplete()
    {
        return true;
    }

    public override IEnumerator TurnLogic()
    {
        if (isDead())
            yield break;

        if (currentCountdown <= 0)
        {
            // do a turn!
            currentCountdown = turnCountdown;

            for (int i = 0; i < tileCount; ++i)
            {
                // pick a node to move toward
                MapTile nextTile = PickATile();
                nextTile.onArriveAtTile += OnMoveToTile;

                // direction
                direction = GetDirectionBetween(MapUnit.CurrentTile.transform.position, nextTile.transform.position);

                MapUnit.CurrentTile = nextTile;
                recentlyVisited.Add(MapUnit.CurrentTile);
                if (recentlyVisited.Count >= backtrackHistorySize)
                    recentlyVisited.RemoveAt(0);
            }
        }

        currentCountdown--;

        yield return null;
    }

    private Direction GetDirectionBetween(Vector3 pos1, Vector3 pos2)
    {
        float diffX = pos1.x - pos2.x;
        float diffY = pos1.y - pos2.y;

        if (diffX < 0)
        {
            return Direction.Right;
        }
        else if (diffX > 0)
        {
            return Direction.Left;
        }

        if (diffY < 0)
        {
            return Direction.Up;
        }
        else if (diffY > 0)
        {
            return Direction.Down;
        }

        return Direction.Up;
    }

    private MapTile PickATile()
    {
        // start by considering our current tile (the current tile may have become the most beneficial tile)
        float currentMapTileScore = CalculateMapTileScore(MapUnit.CurrentTile);
        MapTile nextTile = MapUnit.CurrentTile;

        foreach (MapTile mapTile in MapUnit.CurrentTile.Neighbors)
        {
            if (!mapTile.isValid)
                continue; // don't consider it if it's not valid

            float score = CalculateMapTileScore(mapTile);
            if (score < currentMapTileScore)
            {
                currentMapTileScore = score;
                nextTile = mapTile;
            }
        }

        return nextTile;
    }

    private void OnMoveToTile(MapTile mapTile, MapUnit mapUnit)
    {
        mapTile.onArriveAtTile -= OnMoveToTile;

        MapUnit playerUnit = player.MapUnit;
        MapTile playerTile = playerUnit.CurrentTile;

        if (MapUnit.CurrentTile == playerTile)
        {
            if (player.Inventory.RemoveItem(ItemType.Stake, 1))
            {
                // this enemy dies!
                Die();
            }
            else
            {
                player.Die();
            }
        }
    }

    private float CalculateMapTileScore(MapTile mapTile)
    {
        float score = 0;

        // how close is this tile to the player?
        score += Vector3.Distance(player.transform.position, mapTile.transform.position);

        // consider is this backtracking?
        if (recentlyVisited.Contains(mapTile))
        {
            score *= 1.7f;
        }

        return score;
    }

    public void Die()
    {
        Dead = true;
        deathSound.Play();
        // TODO change "visual" states
    }

    public bool isDead()
    {
        return Dead;
    }
}
