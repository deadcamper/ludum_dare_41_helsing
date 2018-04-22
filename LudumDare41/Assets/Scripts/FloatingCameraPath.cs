using System.Collections;
using UnityEngine;

public class FloatingCameraPath : MonoBehaviour
{
    public float timeLength = 5;

    private Vector3 originalPosition;
    
    void Start()
	{
        StartCoroutine(FloatPath());
        originalPosition = transform.position;
    }	

	IEnumerator FloatPath()
    {
        while (true)
        {
            yield return LerpWFormula(new Vector3(45, 12, 0));
        }
    }

    IEnumerator LerpWFormula(Vector3 endPos)
    {
        float timeCounter, timeSin, timeCos;
        timeCounter = 0f;
        while (timeCounter <= 1)
        {
            timeCounter += (Time.deltaTime / timeLength);
            timeSin = Mathf.Sin(timeCounter * Mathf.PI * 2);
            timeCos = Mathf.Cos(timeCounter * Mathf.PI * 2);

            float x = Mathf.LerpUnclamped(0, 1, timeCos);
            float y = Mathf.LerpUnclamped(0, 1, timeSin);

            transform.position = originalPosition + new Vector3((x * endPos.x) - endPos.x/2, (y * endPos.y) - endPos.y / 2, 0);
            yield return null;
        }
    }
}