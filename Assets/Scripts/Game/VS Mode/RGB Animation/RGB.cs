using UnityEngine;

public class RGB : MonoBehaviour
{
    public SpriteRenderer Target_sprite;
    private float hue = 0f;
    private float hsvTimer = 0f;
    private float cycleDuration = 1f;
    private bool isCycling = false;

    private void Start()
    {
        StartHSVOutlineCycle();
    }

    void Update()
    {
        if (isCycling)
        {
            hsvTimer += Time.deltaTime;
            hue += Time.deltaTime / cycleDuration;
            if (hue > 1f) hue = 0f;

            Target_sprite.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }

    public void StartHSVOutlineCycle()
    {
        hue = 0f;
        hsvTimer = 0f;
        isCycling = true;
    }
}
