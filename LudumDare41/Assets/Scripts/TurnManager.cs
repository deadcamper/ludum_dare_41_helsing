using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<TurnTaker> TurnTakers { get; private set; }
    public bool ShouldRunTurns { private get; set; }

    void Start()
    {
        ShouldRunTurns = true;
        TurnTakers = new List<TurnTaker>(FindObjectsOfType<TurnTaker>());
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
        }
    }
}
