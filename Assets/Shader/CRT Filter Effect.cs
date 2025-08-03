using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CRTFilterEffect : MonoBehaviour
{
    public Material crtMaterial;
    public static bool CRT_state;
    private const string CRT_KEY = "CRT_OFF";

    [Range(0f, 1f)]
    public float scanlineIntensity = 0.3f;

    public float scanlineFrequency = 600f;

    [Range(0f, 0.01f)]
    public float rgbSplitOffset = 0.001f;

    [Header("Screen Movement")]
    public float movementAmplitude = 0.002f; // Small wobble
    public float movementFrequency = 1f; // How fast the screen drifts

    private float _time;

    public void Awake()
    {
        LoadStates();
        Debug.Log("CRT Filter state loaded: " + CRT_state); 
    }

    public static void LoadStates()
    {
        CRT_state = PlayerPrefs.GetInt(CRT_KEY, 0) == 1;
    }

    public static void SaveStates()
    {
        PlayerPrefs.SetInt(CRT_KEY, CRT_state ? 1 : 0);
        PlayerPrefs.Save();
    }

    void Update()
    {
        _time += Time.deltaTime;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (crtMaterial != null)
        {
            crtMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
            crtMaterial.SetFloat("_ScanlineFrequency", scanlineFrequency);
            crtMaterial.SetFloat("_RGBOffset", rgbSplitOffset);

            // Pass movement offset as a Vector2
            Vector2 offset = new Vector2(
                Mathf.Sin(_time * movementFrequency) * movementAmplitude,
                Mathf.Cos(_time * movementFrequency) * movementAmplitude * 0.5f
            );

            crtMaterial.SetVector("_ScreenOffset", offset);

            Graphics.Blit(src, dest, crtMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
