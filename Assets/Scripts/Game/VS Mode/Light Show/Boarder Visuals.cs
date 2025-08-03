using System.Collections;
using UnityEngine;

public class BorderVisuals : MonoBehaviour
{
    public SpriteRenderer[] borders;
    public TimerDisplay global_time;
    public Color pulseColor = Color.red;
    public Color snarePulseColor;
    public float pulseDuration = 0.3f; // total time for pulse (fade in + fade out)
    public float hardPulseDuration = 1.62f;
    public float snarePulseDuration = 1.62f;

    private Color originalColor;
    private bool isPulsing = false;

    void Start()
    {
        if (borders.Length > 0)
            originalColor = borders[0].color;
    }

    public void PulseColor()
    {
        if (!isPulsing)
            StartCoroutine(PulseRoutine(pulseColor));
    }

    public void PulseHeavy()
    {
        StopAllCoroutines();
        StartCoroutine(HardPulseRoutine(Color.red)); // Or any distinct color
    }

    private IEnumerator PulseRoutine(Color targetColor)
    {
        isPulsing = true;
        float duration = 0.3f;
        float t = 0f;

        foreach (var border in borders)
            border.color = targetColor;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            foreach (var border in borders)
            {
                border.color = Color.Lerp(targetColor, originalColor, lerp);
            }
            yield return null;
        }

        isPulsing = false;
    }

    private IEnumerator HardPulseRoutine(Color targetColor)
    {
        isPulsing = true;
        float duration = hardPulseDuration;
        float t = 0f;

        foreach (var border in borders)
            border.color = targetColor;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            foreach (var border in borders)
            {
                border.color = Color.Lerp(targetColor, originalColor, lerp);
            }
            yield return null;
        }

        isPulsing = false;
    }

    public void PulseSnare()
    {
        StopAllCoroutines();
        StartCoroutine(SnarePulseRoutine());
    }

    private IEnumerator SnarePulseRoutine()
    {
        Color snareColor = snarePulseColor;
        float duration = snarePulseDuration;
        float t = 0f;

        foreach (var border in borders)
            border.color = snareColor;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            foreach (var border in borders)
            {
                border.color = Color.Lerp(snareColor, originalColor, lerp);
            }
            yield return null;
        }
    }

}
