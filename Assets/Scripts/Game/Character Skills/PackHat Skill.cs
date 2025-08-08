using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PackHatSkill : MonoBehaviour
{
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public Animator packhatAnim;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;


    [Header("Cooldown Settings")]
    public float cooldownTime = 40f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;
    private void Start()
    {
        cooldownTimer = cooldownTime + 20;
    }

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager.isPlayer1)
        {
            gameManager = GameObject.Find("Game Manager P1").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
        }
        else
        {
            gameManager = GameObject.Find("Game Manager P2").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 2").GetComponent<PlayerInput>();
        }
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
            }
        }
    }

    public void ActivateSkill()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            //AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return;
            
        }
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }

}