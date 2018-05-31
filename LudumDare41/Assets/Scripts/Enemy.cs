using Assets.Scripts.PathFinding;
using Assets.Scripts.PathFinding.Strategies;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : TurnTaker, Killable
{
    public static bool BYPASS_TURN_DURATION = false;
	public const float TURN_DURATION = .12f;

    public class PathAnimationEntry
	{
		public Vector3 goalPosition;

		public Direction goalDirection;
		public int turns;

		public PathAnimationEntry(MapTile goalTile, Direction goalDirection, int turns = 1)
		{
			this.goalPosition = goalTile.transform.position;
			this.goalDirection = goalDirection;
			this.turns = turns;
		}
	}

	private int backtrackHistorySize = 4;
    public int turnCountdown;
    public int tileCount = 1;
    public float playerWeight = 1.0f;
    public float backtrackingWeight = 1.7f;
    private int currentCountdown;
    public MapUnit MapUnit { get; private set; }
    private List<MapTile> recentlyVisited = new List<MapTile>();
    private List<PathAnimationEntry> pathAnimationQueue = new List<PathAnimationEntry>();
    private PathAnimationEntry currentPathTarget;

    public AudioSource deathSound;

	public GameObject liveSprite;
	public GameObject deadSprite;

	[Range(0,10)]
    public int turnsToTurn = 0;

	public Direction startingDirection;

	public bool ignoresWalls = false;

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
        direction = startingDirection;
        currentCountdown = turnCountdown;
		Dead = false;
	}

	float animationStartTime;
	float animationEndTime;
	float startAngle;
	float endAngle;
	Vector3 startPosition;
	Vector3 endPosition;

	private void Update()
	{
		if (pathAnimationQueue.Count > 0)
		{
			if (animationEndTime < Time.time)
			{
				int index = 0;
				currentPathTarget = pathAnimationQueue[index];
				pathAnimationQueue.RemoveAt(index);
				direction = currentPathTarget.goalDirection;

				animationStartTime = Time.time;

				float distance = (((transform.position - Player.Instance.transform.position).magnitude / MapTile.TILE_SIZE));
				float turnDuration = Enemy.TURN_DURATION / Mathf.Max(1, distance/2f);
				if (turnDuration <= 0)
					turnDuration = .01f;

				if (currentPathTarget.turns > 0)
					animationEndTime = animationStartTime + turnDuration / currentPathTarget.turns;
				else
					animationEndTime = animationStartTime;

				startAngle = transform.rotation.eulerAngles.z;
				endAngle = DirectionUtil.GetSpriteRotationForDirection(currentPathTarget.goalDirection).eulerAngles.z;
				startPosition = transform.position;
				endPosition = currentPathTarget.goalPosition;
			}
		}

        if (currentPathTarget != null)
        {
			float lerp = Mathf.InverseLerp(animationStartTime, animationEndTime, Time.time);
			transform.position = Vector3.Lerp(startPosition, endPosition, lerp);
            transform.rotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(startAngle, endAngle, lerp));
        }
    }

    public override bool TurnComplete()
    {
        return (currentPathTarget == null) ? true : (pathAnimationQueue.Count == 0);
    }

    public override IEnumerator TurnLogic()
    {
		if (isDead())
            yield break;

		Direction dir = direction;
		if (currentCountdown <= 0)
		{
			// do a turn!
			currentCountdown = turnCountdown;

			for (int tilesRemaining = tileCount; tilesRemaining > 0; --tilesRemaining)
			{
                //Nah, we're good. We found the player
                if (player.MapUnit.CurrentTile == MapUnit.CurrentTile)
                    break;

                // pick a node to move toward
                MapTile nextTile = PickATile();

                // direction
                if (nextTile != MapUnit.CurrentTile)
                {
                    Direction newDirection = DirectionUtil.GetDirectionBetween(MapUnit.CurrentTile.transform.position, nextTile.transform.position);
                    if (newDirection != dir)
                    {
						dir = newDirection;
                        if (turnsToTurn > 0)
                        {
                            tilesRemaining -= turnsToTurn - 1;
                            if (BYPASS_TURN_DURATION)
                            {
                                transform.position = nextTile.transform.position;
                                pathAnimationQueue.Clear();
                            }
                            else
                            {
                                pathAnimationQueue.Add(new PathAnimationEntry(MapUnit.CurrentTile, dir, Mathf.Min(tilesRemaining, turnsToTurn)));
                            }
							continue;
                        }
                    }
                }
                
                pathAnimationQueue.Add(new PathAnimationEntry(nextTile, dir));
				MapUnit.CurrentTile = nextTile;
				recentlyVisited.Add(MapUnit.CurrentTile);
				if (recentlyVisited.Count >= backtrackHistorySize)
					recentlyVisited.RemoveAt(0);
			}
		}

        currentCountdown--;

		//yield return null;
	}

	private AStar<MapTile, Direction>.Evaluator pathFinder = new AStar<MapTile, Direction>.Evaluator(new SimpleAStar());
    private MapTile PickATile()
    {
		AStar<MapTile, Direction>.Path path;
		if(pathFinder.FindPath(MapUnit.CurrentTile, out path))
		{
			MapTile lastMapTile = null;
			foreach (MapTile mapTile in path.FullPath)
			{
				if (lastMapTile != null)
				{
					Debug.DrawLine(lastMapTile.transform.position - Vector3.forward, mapTile.transform.position - Vector3.forward,Color.red);
				}
				lastMapTile = mapTile;
			}

			return path.PlannedStates.FirstOrDefault();
		}

		// start by considering our current tile (the current tile may have become the most beneficial tile)
		float currentMapTileScore = CalculateMapTileScore(MapUnit.CurrentTile, null);
        MapTile nextTile = MapUnit.CurrentTile;

        foreach (Direction tileDirection in DirectionUtil.All)
        {
			MapTile mapTile = MapUnit.CurrentTile.GetNeighbor(tileDirection);

            if (mapTile == null)
                continue;

			if (!mapTile.isValid && !ignoresWalls)
                continue; // don't consider it if it's not valid

			if(mapTile.tileType == TileType.Wall && !ignoresWalls)
				continue;

			if (IsTileOccupied(mapTile))
                continue;

            float score = CalculateMapTileScore(mapTile, tileDirection);
			
            if (score < currentMapTileScore)
            {
                currentMapTileScore = score;
                nextTile = mapTile;
            }
        }

        return nextTile;
    }

    private bool IsTileOccupied(MapTile mapTile)
    {
        Enemy[] turnTakers = FindObjectsOfType<Enemy>();

        foreach (Enemy tt in turnTakers)
        {
            if (tt == this)
                continue;
            if (tt.MapUnit.CurrentTile == mapTile)
                return true;
        }
        return false;
    }

    private float CalculateMapTileScore(MapTile mapTile, Direction? tileDirection)
    {
        float score = 0;

        // how close is this tile to the player?
        score += Vector3.Distance(player.transform.position, mapTile.transform.position) * playerWeight;

		if (!tileDirection.HasValue || tileDirection.Value != direction)
		{
            score += 32f * playerWeight;
		}

		// consider is this backtracking?
		if (recentlyVisited.Contains(mapTile))
        {
            score *= backtrackingWeight;
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
