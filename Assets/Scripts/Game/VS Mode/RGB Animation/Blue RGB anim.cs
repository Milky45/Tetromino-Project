using UnityEngine;

public class BlueRGB : MonoBehaviour
{
    public SpriteRenderer Target_sprite;

    private float hue = 0.6667f;
    private float hueMin = 0.6667f;
    private float hueMax = 0.8333f;
    public float hueSpeed = 0.05f; // Speed of hue transition
    private bool isIncreasing = true;
    private bool isCycling = false;

    void Start()
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
