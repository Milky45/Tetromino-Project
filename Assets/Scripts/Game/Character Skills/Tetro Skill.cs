using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class TetroSkill : MonoBehaviour
{
    [Header("References")]
    public CharacterManager characterManager;
    public Game_Manager gameManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Blinding Overlay")]
    
    public GameObject blindOverlay;
    public GameObject blindBall;
    public Animator animBall;
    public SpriteRenderer BO_Renderer;
    public SpriteRenderer ballRenderer;
        
    [Header("Cooldown Settings")]
    public float cooldownTime = 25f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public GameDisplay gameDisplay;
    private Coroutine pulseRoutine;

    public int cost = 300;


    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager.isPlayer1)
        {
            blindBall = GameObject.Find("Blind Ball P1");
            animBall = blindBall.GetComponent<Animator>();
            ballRenderer = blindBall.GetComponent<SpriteRenderer>();
            blindOverlay = GameObject.Find("Blind Overlay P1");
            BO_Renderer = blindOverlay.GetComponent<SpriteRenderer>();

            gameManager = GameObject.Find("Game Manager P1").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
        }
        else
        {
            blindBall = GameObject.Find("Blind Ball P2");
            animBall = blindBall.GetComponent<Animator>();
            ballRenderer = blindBall.GetComponent<SpriteRenderer>();
            blindOverlay = GameObject.Find("Blind Overlay P2");
            BO_Renderer = blindOverlay.GetComponent<SpriteRenderer>();

            gameManager = GameObject.Find("Game Manager P2").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 2").GetComponent<PlayerInput>();
        }

        if (blindOverlay != null || blindBall != null || animBall != null || BO_Renderer != null || ballRenderer != null)
        {
            BO_Renderer.enabled = false;
            ballRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("One or more required components are missing. Please check the GameObject setup.");
        }

        skillAction = playerInput.actions.FindAction("Skill");
        skillAction.performed += ctx => BallAnim();

        // Start cooldown at the beginning
        isOnCooldown = true;
        cooldownTimer = cooldownTime + 35;

        gameDisplay.costText.text = cost.ToString();
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            gameDisplay.SkillCooldownUpdate(cooldownTimer);

            // Turn off cooldown when timer ends
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
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
            //AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return;
        }
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }
        if (gameManager.isTimeStopped) return;
        
        ballRenderer.enabled = true;
        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);
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
        BO_Renderer.enabled = true;
        ballRenderer.enabled = false;
        Color baseColor = BO_Renderer.color;
        
        // Step 1: Set to full opacity
        BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        // Hold full opacity for 3 seconds
        yield return new WaitForSeconds(5f);
        animBall.SetTrigger("Return");
        

        // Step 2: Fade out over 3 seconds
        float fadeDuration = 2f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("FadeOut");
        BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        BO_Renderer.enabled = false;
        
        
    }
}
