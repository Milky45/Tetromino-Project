using UnityEngine;
using UnityEngine.UI;

public class CRTFilter : MonoBehaviour
{
    public CRTFilterEffect crtFilter;
    public Toggle toggleUI;

    private void Start()
    {
        // Load saved CRT state
        CRTFilterEffect.LoadStates();

        // Apply saved state
        bool isCRTOn = CRTFilterEffect.CRT_state;
        crtFilter.enabled = isCRTOn;

        // Sync UI toggle (true = CRT OFF)
        if (toggleUI != null)
        {
            toggleUI.isOn = !isCRTOn;
        }
    }

    public void OnClickToggle()
    {
        if (crtFilter != null)
        {
            // If toggle is ON, user wants CRT OFF (inverse logic)
            bool shouldDisableCRT = toggleUI.isOn;
            crtFilter.enabled = !shouldDisableCRT;

            // Save correct state
            CRTFilterEffect.CRT_state = crtFilter.enabled;
            CRTFilterEffect.SaveStates();

            Debug.Log("CRT Filter toggled: " + crtFilter.enabled);
        }
        else
        {
            Debug.LogWarning("CRT Filter not assigned.");
        }
    }
}
