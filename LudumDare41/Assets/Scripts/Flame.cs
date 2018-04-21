using UnityEngine;

public class Flame : MonoBehaviour
{
    public float flicker;
    float cooldown = 0;
    public SpriteRenderer spriteRenderer;
    private bool scaleUp = true;

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;

        if (cooldown <= 0.0f)
        {
            cooldown = flicker;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        if (scaleUp)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.7f, Time.deltaTime);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f, Time.deltaTime);

        if (transform.localScale.x > 0.65f) scaleUp = false;
        if (transform.localScale.x < 0.55f) scaleUp = true;

        spriteRenderer.color = new Color(1, 1, 1, transform.localScale.x);
    }
}
