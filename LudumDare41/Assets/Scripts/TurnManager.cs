using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    //public List<TurnTaker> TurnTakers { get; private set; }
    public bool ShouldRunTurns { private get; set; }

    private List<TurnTaker> TurnTakers;
    private List<TurnTaker> TurnTakersToQueue = new List<TurnTaker>();

    private static int Compare(TurnTaker turn1, TurnTaker turn2)
    {
        //Player is always last
        if (turn1.GetComponent<Player>() != null)
            return 1;
        if (turn2.GetComponent<Player>() != null)
            return -1;
        return 0;
    }

    void Start()
    {
        ShouldRunTurns = true;
        TurnTakers = new List<TurnTaker>(FindObjectsOfType<TurnTaker>());
        TurnTakers.Sort(Compare);
        StartCoroutine(RunTurns());
    }


    IEnumerator RunTurns()
    {
        while (ShouldRunTurns)
        {
            foreach (TurnTaker turnTaker in TurnTakers)
            {
                StartCoroutine(turnTaker.TurnLogic());
                do
                {
                    yield return null; // wait for the turn to be done
                } while (!turnTaker.TurnComplete());
            }

            if (TurnTakersToQueue.Count > 0)
            {
                TurnTakers.AddRange(TurnTakersToQueue);
                TurnTakers.Sort(Compare);
                TurnTakersToQueue.Clear();
            }
        }
    }

    public void AddTurnTaker(TurnTaker turnTaker)
    {
        TurnTakersToQueue.Add(turnTaker);
    }
}
