using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.PathFinding.Strategies
{
	public class SimpleAStar : AStar<MapTile, Direction>.IStrategy
	{
		struct Move
		{
			public readonly Direction direction;
			public Move(Direction direction)
			{
				this.direction = direction;
			}
		}

		public float HeuristicCostEstimate(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState, MapTile node)
		{
			return Vector2Int.Distance(Player.Instance.MapUnit.CurrentTile.Coordinates, node.Coordinates, Vector2Int.DistanceType.Manhattan) + UnityEngine.Random.Range(0,.1f);
		}

		public bool MeetsGoals(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState, MapTile node)
		{
			return Player.Instance.MapUnit.CurrentTile.Coordinates.Equals(node.Coordinates);
		}

		public IEnumerable<Direction> GetNextSteps(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState, MapTile state)
		{
			return DirectionUtil.All;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="action"></param>
		/// <param name="cost"></param>
		/// <param name="resultingState"></param>
		/// <returns>true if valid action</returns>
		public bool EvalAction(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState, MapTile state, Direction action, out float cost, out MapTile resultingState)
		{
			cost = 1;
			resultingState = null;
			MapTile neighbor = state.GetNeighbor(action);
			if (neighbor != null && neighbor.isValid)
			{
				resultingState = neighbor;
				return true;
			}
			return false;
		}

		public bool TreatAsFailure(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState)
		{
			return false;
		}

		public bool TreatAsSuccess(AStar<MapTile, Direction>.EvaluatorState evaluatorState, MapTile startingState, out AStar<MapTile, Direction>.Path path)
		{
			path = default(AStar<MapTile, Direction>.Path);
			return false;
		}
	}
}
