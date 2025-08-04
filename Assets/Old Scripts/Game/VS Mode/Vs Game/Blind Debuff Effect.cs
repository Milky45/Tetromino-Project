using UnityEngine;

[RequireComponent(typeof(CanvasRenderer))]
public class BlindingDebuffEffect : MonoBehaviour
{
    public Material crtMaterial; // Assign the same material used by your CRT effect
    public float duration = 1.5f;

    [Header("Animation Targets")]
    public float targetScanlineIntensity = 1.0f;
    public float targetRGBOffset = 0.01f;
    public float animationSpeed = 10f;

    private float originalScanlineIntensity;
    private float originalRGBOffset;
    private float timer;
    private bool isActive;

    void Start()
    {
        if (crtMaterial != null)
        {
            originalScanlineIntensity = crtMaterial.GetFloat("_ScanlineIntensity");
            originalRGBOffset = crtMaterial.GetFloat("_RGBOffset");
        }
    }

    public void TriggerDebuff()
    {
        if (!isActive)
        {
            timer = 0f;
            isActive = true;
        }
    }

    void Update()
    {
        if (!isActive || crtMaterial == null) return;

        timer += Time.deltaTime;

        // Animate IN
        if (timer < duration / 2f)
        {
            float t = timer / (duration / 2f);
            crtMaterial.SetFloat("_ScanlineIntensity", Mathf.Lerp(originalScanlineIntensity, targetScanlineIntensity, t));
            crtMaterial.SetFloat("_RGBOffset", Mathf.Lerp(originalRGBOffset, targetRGBOffset, t));
        }
        // Animate OUT
        else if (timer < duration)
        {
            float t = (timer - duration / 2f) / (duration / 2f);
            crtMaterial.SetFloat("_ScanlineIntensity", Mathf.Lerp(targetScanlineIntensity, originalScanlineIntensity, t));
            crtMaterial.SetFloat("_RGBOffset", Mathf.Lerp(targetRGBOffset, originalRGBOffset, t));
        }
        else
        {
            // Reset
            crtMaterial.SetFloat("_ScanlineIntensity", originalScanlineIntensity);
            crtMaterial.SetFloat("_RGBOffset", originalRGBOffset);
            isActive = false;
        }
    }
}
