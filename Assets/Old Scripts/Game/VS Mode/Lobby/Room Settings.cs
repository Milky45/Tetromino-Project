using TMPro;
using UnityEngine;

public class RoomSettings : MonoBehaviour
{
    [Header("References")]
    public GameObject CharSelect;
    public TextMeshProUGUI Gravity_Text;
    public TextMeshProUGUI AtkCD_Text;
    public TextMeshProUGUI EMP_CD_Text;
    public TextMeshProUGUI CRT_Text;
    public GameObject VsSetup;
    public GameObject button;
    public CRTFilterEffect crtFilter;
    

    // Settings options and values
    private readonly string[] gravityOptions = { "Easy", "Normal", "Hard", "Insane", "Crazy" };
    private readonly float[] gravityDelays = { 0.8f, 0.7f, 0.6f, 0.5f, 0.4f };

    private readonly string[] attackCooldownOptions = {
        "No Cooldown", "1 sec", "1.5 sec", "2 sec", "2.5 sec",
        "3 sec", "3.5 sec", "4 sec", "4.5 sec", "5 sec"
    };

    private readonly float[] attackCooldowns = {
        0f, 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 4.5f, 5f
    };

    private readonly string[] empOptions = {
        "10 sec", "15 sec", "20 sec", "25 sec", "30 sec",
        "35 sec", "40 sec", "45 sec", "50 sec", "55 sec", "60 sec"
    };
    private readonly float[] empCooldowns = {
        10f, 15f, 20f, 25f, 30f, 35f, 40f, 45f, 50f, 55f, 60f
    };

    // Internal state tracking
    private int gravityIndex = 0; // Default: Easy
    private int atkCDIndex = 2;   // Default: 1.5 sec
    private int empIndex = 4;     // Default: 30 sec
    private bool crtEnabled;

    void Start()
    {
        CRTFilterEffect.LoadStates();
        crtFilter.enabled = CRTFilterEffect.CRT_state;
        // Load the current gravity delay from both game managers
        float p1Gravity = GameManager.initialGravityDelay;
        float p2Gravity = GameManagerP2.initialGravityDelay;
        
        // Find the matching index in gravityDelays array
        bool foundMatch = false;
        for (int i = 0; i < gravityDelays.Length; i++)
        {
            // Check if either P1 or P2's gravity matches
            if (Mathf.Approximately(p1Gravity, gravityDelays[i]) || 
                Mathf.Approximately(p2Gravity, gravityDelays[i]))
            {
                gravityIndex = i;
                foundMatch = true;
                break;
            }
        }
        
        // If no match found, set to Easy (index 0)
        if (!foundMatch)
        {
            gravityIndex = 0;
        }

        // Load the current attack cooldown from both game managers
        float p1AtkCD = GameManager.attackCooldownTime;
        float p2AtkCD = GameManagerP2.attackCooldownTime;
        
        // Find the matching index in attackCooldowns array
        foundMatch = false;
        for (int i = 0; i < attackCooldowns.Length; i++)
        {
            // Check if either P1 or P2's attack cooldown matches
            if (Mathf.Approximately(p1AtkCD, attackCooldowns[i]) || 
                Mathf.Approximately(p2AtkCD, attackCooldowns[i]))
            {
                atkCDIndex = i;
                foundMatch = true;
                break;
            }
        }
        
        // If no match found, set to 1.5 sec (index 2)
        if (!foundMatch)
        {
            atkCDIndex = 2;
        }

        // Load the current EMP cooldown from both game managers
        float p1EmpCD = GameManager.empCooldownDuration;
        float p2EmpCD = GameManagerP2.empCooldownDuration;
        
        // Find the matching index in empCooldowns array
        foundMatch = false;
        for (int i = 0; i < empCooldowns.Length; i++)
        {
            // Check if either P1 or P2's EMP cooldown matches
            if (Mathf.Approximately(p1EmpCD, empCooldowns[i]) || 
                Mathf.Approximately(p2EmpCD, empCooldowns[i]))
            {
                empIndex = i;
                foundMatch = true;
                break;
            }
        }
        
        // If no match found, set to 30 sec (index 4)
        if (!foundMatch)
        {
            empIndex = 4;
        }

        // Load the current CRT filter state
        if (crtFilter != null)
        {
            crtEnabled = crtFilter.enabled;
            UpdateCRTDisplay();
        }
        
        UpdateDisplayTexts();
    }

    public void OnClickLeftGravity() => CycleOption(ref gravityIndex, gravityOptions.Length, -1, Gravity_Text);

    public void OnClickRightGravity() => CycleOption(ref gravityIndex, gravityOptions.Length, 1, Gravity_Text);

    public void OnClickLeftAtkCD() => CycleOption(ref atkCDIndex, attackCooldownOptions.Length, -1, AtkCD_Text);

    public void OnClickRightAtkCD() => CycleOption(ref atkCDIndex, attackCooldownOptions.Length, 1, AtkCD_Text);

    public void OnClickLeftEMP_CD() => CycleOption(ref empIndex, empOptions.Length, -1, EMP_CD_Text);

    public void OnClickRightEMP_CD() => CycleOption(ref empIndex, empOptions.Length, 1, EMP_CD_Text);

    public void OnClickLeftCRT()
    {
        crtEnabled = !crtEnabled;
        if (crtFilter != null)
        {
            crtFilter.enabled = crtEnabled;
        }
        UpdateCRTDisplay();
    }

    public void OnClickRightCRT()
    {
        crtEnabled = !crtEnabled;
        if (crtFilter != null)
        {
            crtFilter.enabled = crtEnabled;
        }
        UpdateCRTDisplay();
    }

    private void CycleOption(ref int index, int length, int direction, TextMeshProUGUI display)
    {
        index = (index + direction + length) % length;
        UpdateDisplayTexts();
    }

    private void UpdateDisplayTexts()
    {
        Gravity_Text.text = gravityOptions[gravityIndex];
        AtkCD_Text.text = attackCooldownOptions[atkCDIndex];
        EMP_CD_Text.text = empOptions[empIndex];
    }

    private void UpdateCRTDisplay()
    {
        CRT_Text.text = crtEnabled ? "On" : "Off";
        CRTFilterEffect.CRT_state = crtEnabled;
        CRTFilterEffect.SaveStates();
    }

    public void OnClickSubmit()
    {
        float gravity = gravityDelays[gravityIndex];
        float attackCD = attackCooldowns[atkCDIndex];
        float empCD = empCooldowns[empIndex];

        // Apply to both players
        GameManager.initialGravityDelay = gravity;
        GameManagerP2.initialGravityDelay = gravity;

        GameManager.attackCooldownTime = attackCD;
        GameManagerP2.attackCooldownTime = attackCD;

        GameManager.empCooldownDuration = empCD;
        GameManagerP2.empCooldownDuration = empCD;

        Debug.Log($"Settings Applied:\nGravity = {gravity}s\nAttack Cooldown = {attackCD}s\nEMP Cooldown = {empCD}s");
    }

    public void OnClickBack()
    {
        button.SetActive(true);
        VsSetup.SetActive(false);
        CharSelect.SetActive(true);
    }

    public void OnClickSettings()
    {
        button.SetActive(false);
        VsSetup.SetActive(true);
        CharSelect.SetActive(false);
    }
}
