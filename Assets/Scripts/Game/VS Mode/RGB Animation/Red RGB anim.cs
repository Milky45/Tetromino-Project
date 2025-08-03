using UnityEngine;

public class RedRGB : MonoBehaviour
{
    public SpriteRenderer Target_sprite;

    private float hue = 0.0027f; // Start at ~1 degree
    private float hueMin = 0.0027f;
    private float hueMax = 0.1f; // ~36 degrees
    public float hueSpeed = 0.05f; // Speed of hue change
    private bool isIncreasing = true;
    private bool isCycling = false;

    private void Start()
    {
        StartHSVOutlineCycle();
    }

    void Update()
    {
        if (isCycling)
        {
            if (isIncreasing)
            {
                hue += Time.deltaTime * hueSpeed;
                if (hue >= hueMax)
                {
                    hue = hueMax;
                    isIncreasing = false;
                }
            }
            else
            {
                hue -= Time.deltaTime * hueSpeed;
                if (hue <= hueMin)
                {
                    hue = hueMin;
                    isIncreasing = true;
                }
            }

            Target_sprite.color = Color.HSVToRGB(hue, 1f, 1f);
        }
    }

    public void StartHSVOutlineCycle()
    {
        hue = hueMin;
        isIncreasing = true;
        isCycling = true;
    }
}
