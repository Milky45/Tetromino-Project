using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DodokeSkill : MonoBehaviour
{
    [Header("Board Target")]
    public Transform MainTileMap;
    public Transform GhostTileMap;
    public float SkillDuration = 10f;
    public float Temp_Z_Position;

    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText; // <-- Assign in inspector
    private string cooldownTempText;
    private string originalText;

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

    public void ActivateSkill()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return;

        }
        Debug.Log("Dodoke skill activated!");
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        StartCoroutine(BoardFlip());
    }

    public IEnumerator BoardFlip()
    {
        Debug.Log("Board flip started!");

        // Store original position and rotation
        Vector3 originalPosition = MainTileMap.position;
        Quaternion originalRotation = MainTileMap.rotation;
        Quaternion ghostOriginalRotation = GhostTileMap.rotation;

        // Flip Z position and rotation
        MainTileMap.position = new Vector3(originalPosition.x, originalPosition.y, -5f);
        MainTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);

        if (GhostTileMap != null)
        {
            GhostTileMap.position = MainTileMap.position;
            GhostTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);
        }

        yield return new WaitForSeconds(SkillDuration);

        // Restore position and rotation
        MainTileMap.position = originalPosition;
        MainTileMap.rotation = originalRotation;

        if (GhostTileMap != null)
        {
            GhostTileMap.position = originalPosition;
            GhostTileMap.rotation = ghostOriginalRotation;
        }

        Debug.Log("Board flip ended!");
    }

}
