using UnityEngine;
using UnityEngine.UI;

public class Glow : MonoBehaviour
{
    private bool toggle = true;

    public Image image;
    public float flicker;

    // Update is called once per frame
    void Update()
    {
        if (toggle)
        {
            image.color = Color.Lerp(image.color, new Color(1, 1, 1, 0.7f), flicker);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, flicker);

            if (transform.localScale.x <= 0.7f)
            {
                toggle = false;
            }
        }
        else
        {
            image.color = Color.Lerp(image.color, new Color(1, 1, 1, 0.5f), flicker);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.2f, flicker);

            if (transform.localScale.x >= 1.1f)
            {
                toggle = true;
            }
        }
    }
}
