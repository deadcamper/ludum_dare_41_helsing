using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer spr;
    private Vector3 initialScale;

    private void Awake()
    {
        spr.color = Color.clear; // start as not visible
        initialScale = transform.localScale;
    }

    public void Fire()
    {
        spr.color = Color.white;
        transform.localScale = initialScale;
    }

    // Update is called once per frame
    void Update()
    {
        spr.color = new Color(1, 1, 1, spr.color.a - 4f * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, initialScale * 2.5f, 3f * Time.deltaTime);
    }
}
