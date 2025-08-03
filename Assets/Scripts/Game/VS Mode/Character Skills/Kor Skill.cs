using TMPro;
using UnityEngine;

public class KorSkill : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText;
    private string cooldownTempText;
    private string originalText;
    private Color originalTextColor;

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
                cooldownText.text = "OK"; // Should be "0"
                cooldownText.color = Color.green; // Restore original color
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;

                if (cooldownText != null)
                {
                    cooldownText.text = originalText; // Should be "0"
                    cooldownText.color = originalTextColor; // Restore original color
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

}
