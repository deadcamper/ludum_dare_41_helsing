using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.PathFinding
{
	public static class AStar<TState, TAction>
	{

		public struct StateAction
		{
			public readonly TAction action;
			public readonly TState state;
			public StateAction(TState state, TAction action)
			{
				this.action = action;
				this.state = state;
			}
		}

		public interface IStrategy
		{
			IEnumerable<TAction> GetNextSteps(EvaluatorState evaluatorState, TState startingState, TState state);

			bool EvalAction(EvaluatorState evaluatorState, TState startingState, TState state, TAction action, out float cost, out TState resultingState);

			//controls evaluation order biases
			float HeuristicCostEstimate(EvaluatorState evaluatorState, TState startingState, TState state);

			bool MeetsGoals(EvaluatorState evaluatorState, TState startingState, TState state);

			bool TreatAsFailure(EvaluatorState evaluatorState, TState startingState);
			bool TreatAsSuccess(EvaluatorState evaluatorState, TState startingState, out Path path);
		}

		public class Path
		{
			public readonly TState StartNode;

			public readonly IEnumerable<TState> PlannedStates;

			public readonly IEnumerable<TAction> PlannedActions;

			public IEnumerable<TState> FullPath
			{
				get
				{
					yield return StartNode;
					foreach (TState node in PlannedStates)
						yield return node;
				}
			}

			public Path(TState StartNode, IEnumerable<TState> PlannedStates, IEnumerable<TAction> PlannedActions)
			{
				this.StartNode = StartNode;
				this.PlannedStates = PlannedStates;
				this.PlannedActions = PlannedActions;
			}
		}

		public class EvaluatorState
		{
			// The set of nodes already evaluated
			public readonly HashSet<TState> closedSet = new HashSet<TState>();

			// The set of currently discovered nodes that are not evaluated yet.
			// Initially, only the start node is known.
			public readonly HashSet<TState> openSet = new HashSet<TState>();

			// For each node, which node it can most efficiently be reached from.
			// If a node can be reached from many nodes, cameFrom will eventually contain the
			// most efficient previous step.
			public readonly Dictionary<TState, StateAction> cameFrom = new Dictionary<TState, StateAction>();

			// For each node, the cost of getting from the start node to that node.
			public IDictionary<TState, float> gScore = new SparseDictionary<TState, float>(float.PositiveInfinity);// map with default value of Infinity;

			// For each node, the total cost of getting from the start node to the goal
			// by passing by that node. That value is partly known, partly heuristic.
			public IDictionary<TState, float> fScore = new SparseDictionary<TState, float>(float.PositiveInfinity); // map with default value of Infinity


			public void Clear()
			{
				closedSet.Clear();
				openSet.Clear();
				cameFrom.Clear();
				gScore.Clear();
				fScore.Clear();
			}

			public Path GetOptimalPathTo(TState state)
			{
				Debug.Assert(cameFrom.ContainsKey(state));

				TState currentState = state;
				List<TState> optimalStates = new List<TState>();
				List<TAction> optimalActions = new List<TAction>();

				optimalStates.Add(currentState);
				while (cameFrom.ContainsKey(currentState))
				{
					var currentStep = cameFrom[currentState];
					optimalStates.Add(currentStep.state);
					optimalActions.Add(currentStep.action);
					currentState = currentStep.state;
				}

				optimalStates.Reverse();
				optimalActions.Reverse();

				return new Path(optimalStates[0], optimalStates.Skip(1), optimalActions);
			}
		}
		public class Evaluator
		{
			IStrategy strategy;

			EvaluatorState evaluatorState = new EvaluatorState();

			Dictionary<TState, Tuple<TAction, float>> possibles = new Dictionary<TState, Tuple<TAction, float>>();

			public Evaluator(IStrategy strategy)
			{
				this.strategy = strategy;
			}

			public bool FindPath(TState startingState, out Path path)
			{
				path = default(Path);

				evaluatorState.Clear();

				var openSet = evaluatorState.openSet;
				var gScore = evaluatorState.gScore;
				var fScore = evaluatorState.fScore;
				var cameFrom = evaluatorState.cameFrom;
				var closedSet = evaluatorState.closedSet;

				openSet.Add(startingState);

				// The cost of going from start to start is zero.
				gScore[startingState] = 0;

				// For the first node, that value is completely heuristic.
				fScore[startingState] = strategy.HeuristicCostEstimate(evaluatorState, startingState, startingState);


				while (openSet.Any())
				{

					if (strategy.TreatAsSuccess(evaluatorState, startingState, out path))
						return true;

					if (strategy.TreatAsFailure(evaluatorState, startingState))
						return false;

					TState current = openSet.Aggregate((l, r) => fScore[l] < fScore[r] ? l : r);//the node in openSet having the lowest fScore[] value

					if (strategy.MeetsGoals(evaluatorState, startingState, current))
					{
						path = evaluatorState.GetOptimalPathTo(current);
						return true;
					}

					openSet.Remove(current);

					closedSet.Add(current);
					possibles.Clear();
					foreach (TAction possibleAction in strategy.GetNextSteps(evaluatorState, startingState, current))
					{
						TState resulting_state;
						float cost;

						if (!strategy.EvalAction(evaluatorState, startingState, current, possibleAction, out cost, out resulting_state))
							continue; // was not a valid action

						if (closedSet.Contains(resulting_state))
							continue;        // Ignore the neighbor which is already evaluated.


						if (!openSet.Contains(resulting_state))  // Discover a new node
							openSet.Add(resulting_state);

						// The distance from start to a neighbor
						//the "dist_between" function may vary as per the solution requirements.
						float tentative_gScore = gScore[current] + cost;
						if (tentative_gScore <= gScore[resulting_state])
							continue;       // This is not a better path.

						Tuple<TAction, float> currentPossible;
						if (possibles.TryGetValue(resulting_state, out currentPossible) && currentPossible.Item2 <= tentative_gScore)
							continue;       // This is not a better path.

						// This path is the best until now. Record it!
						possibles[resulting_state] = Tuple.Create(possibleAction, tentative_gScore);
					}
					foreach (KeyValuePair<TState, Tuple<TAction, float>> possible in possibles)
					{
						TState resulting_state = possible.Key;
						TAction possibleAction = possible.Value.Item1;
						float tentative_gScore = possible.Value.Item2;

						cameFrom[resulting_state] = new StateAction(current, possibleAction);
						gScore[resulting_state] = tentative_gScore;
						fScore[resulting_state] = gScore[resulting_state] + strategy.HeuristicCostEstimate(evaluatorState, startingState, resulting_state);
					}
				}

				if (strategy.TreatAsSuccess(evaluatorState, startingState, out path))
				{
					return true;
				}

				return false;
			}
		}
		private class SparseDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		{
			Dictionary<TKey, TValue> internalDictionary = new Dictionary<TKey, TValue>();
			public readonly TValue defaultValue;

			public SparseDictionary(TValue defaultValue)
			{
				this.defaultValue = defaultValue;
			}

			public TValue this[TKey key]
			{
				get
				{
					TValue value;
					if (internalDictionary.TryGetValue(key, out value))
					{
						return value;
					}
					else
					{
						return defaultValue;
					}
				}

				set
				{
					if (value.Equals(defaultValue))
					{
						Remove(key);
					}
					else
					{
						((IDictionary<TKey, TValue>)internalDictionary)[key] = value;
					}
				}
			}

			public int Count
			{
				get
				{
					return ((IDictionary<TKey, TValue>)internalDictionary).Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return ((IDictionary<TKey, TValue>)internalDictionary).IsReadOnly;
				}
			}

			public ICollection<TKey> Keys
			{
				get
				{
					return ((IDictionary<TKey, TValue>)internalDictionary).Keys;
				}
			}

			public ICollection<TValue> Values
			{
				get
				{
					return ((IDictionary<TKey, TValue>)internalDictionary).Values;
				}
			}

			public void Add(KeyValuePair<TKey, TValue> item)
			{
				if (item.Value.Equals(defaultValue))
					return;

				((IDictionary<TKey, TValue>)internalDictionary).Add(item);
			}

			public void Add(TKey key, TValue value)
			{
				if (value.Equals(defaultValue))
					return;

				((IDictionary<TKey, TValue>)internalDictionary).Add(key, value);
			}

			public void Clear()
			{
				((IDictionary<TKey, TValue>)internalDictionary).Clear();
			}

			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).Contains(item);
			}

			public bool ContainsKey(TKey key)
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).ContainsKey(key);
			}

			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				((IDictionary<TKey, TValue>)internalDictionary).CopyTo(array, arrayIndex);
			}

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).GetEnumerator();
			}

			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).Remove(item);
			}

			public bool Remove(TKey key)
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).Remove(key);
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).TryGetValue(key, out value);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IDictionary<TKey, TValue>)internalDictionary).GetEnumerator();
			}
		}
	}
	
}