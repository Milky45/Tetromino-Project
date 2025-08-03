using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TetroSkill : MonoBehaviour
{
    [Header("Blinding Overlay")]
    public GameObject blindOverlay;
    public GameObject blindBall;
    public Animator animBall;
        
    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText; // <-- Assign in inspector
    private string cooldownTempText;
    private string originalText;
    private Color originalTextColor;

    private Coroutine pulseRoutine;

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

    public void SFX()
    {
        AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.Blind);
    }

    public void BallAnim()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return;
        }
        blindBall.SetActive(true);
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        
        animBall.SetTrigger("Activate");

    }

    public void ActivateSkill()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);
        
        pulseRoutine = StartCoroutine(BlindPulse());
        
    }

    private IEnumerator BlindPulse()
    {
        blindOverlay.SetActive(true);

        Image overlayImage = blindOverlay.GetComponent<Image>();
        SpriteRenderer Ball = blindBall.GetComponent<SpriteRenderer>();

        Color baseColor = overlayImage.color;
        Color ballColor = Ball.color;
        
        // Step 1: Set to full opacity
        overlayImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        Ball.color = new Color(ballColor.r, ballColor.g, ballColor.b, 0f);
        // Hold full opacity for 3 seconds
        yield return new WaitForSeconds(2.5f);
        animBall.SetTrigger("Return");
        

        // Step 2: Fade out over 3 seconds
        float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            overlayImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("FadeOut");
        overlayImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        Ball.color = new Color(ballColor.r, ballColor.g, ballColor.b, 1f);
        blindOverlay.SetActive(false);
        blindBall.SetActive(false);
        
    }


    // Optional method to retrieve the current cooldown string
    public string GetCooldownDisplay()
    {
        return cooldownTempText;
    }
}
