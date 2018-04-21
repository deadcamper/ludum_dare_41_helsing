using System.Collections;
using UnityEngine;

public abstract class TurnTaker : MonoBehaviour
{
    public abstract IEnumerator TurnLogic();
    public abstract bool TurnComplete();
}
