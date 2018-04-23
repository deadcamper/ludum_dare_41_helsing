using System.Collections;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
        StartCoroutine(BounceScale());
    }

    private IEnumerator BounceScale()
    {
        while (gameObject)
        {
            while (transform.localScale.x > initialScale.x * 0.9f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 0.8f, 1f * Time.deltaTime);
                yield return null;
            }

            while (transform.localScale.x < initialScale.x)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 1.2f, 1f * Time.deltaTime);
                yield return null;
            }
        }
    }
}