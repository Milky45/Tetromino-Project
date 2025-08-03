using TMPro;
using UnityEngine;

public class PackHatSkill : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText; // <-- Assign in inspector
    private string cooldownTempText;
    private string originalText;
    private Color originalTextColor;

    public GameObject PackHat_Main;
    public GameObject PackHat_Transform;
    private void Start()
    {
        originalText = cooldownText.text;
        cooldownTimer = cooldownTime;
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            cooldownTempText = Mathf.CeilToInt(cooldownTimer).ToString();

            if (cooldownText != null)
            {
                cooldownText.text = cooldownTempText;
                cooldownText.color = Color.red; // Set to red while counting down
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;

                if (cooldownText != null)
                {
                    cooldownText.text = "OK"; // Should be "0"
                    cooldownText.color = Color.green; // Restore original color
                }
            }
        }
    }

    public bool ActivateSkill()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return false;
            
        }
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        return true;
    }

    public void Transform()
    {
        // Move PackHat_Transform up by 1
        Vector3 transformPos = PackHat_Transform.transform.position;
        transformPos.y = -330.605f;
        PackHat_Transform.transform.position = transformPos;

        // Move PackHat_Main down by 1
        Vector3 mainPos = PackHat_Main.transform.position;
        mainPos.y = -661.21f;
        PackHat_Main.transform.position = mainPos;

    }
    public void NormalState()
    {
        // Move PackHat_Transform down by 1
        Vector3 transformPos = PackHat_Transform.transform.position;
        transformPos.y = -661.21f;
        PackHat_Transform.transform.position = transformPos;

        // Move PackHat_Main up by 1
        Vector3 mainPos = PackHat_Main.transform.position;
        mainPos.y = -330.605f;
        PackHat_Main.transform.position = mainPos;
    }
}