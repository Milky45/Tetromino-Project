using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EthanSkill : MonoBehaviour
{
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public Game_Manager opponent;
    public GameDisplay gameDisplay;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;


    [Header("Cooldown Settings")]
    public float cooldownTime = 65f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    public int cost = 500;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager.isPlayer1)
        {
            gameManager = GameObject.Find("Game Manager P1").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
            opponent = gameManager.pvp.opponentGameManager;


        }
        else
        {
            gameManager = GameObject.Find("Game Manager P2").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 2").GetComponent<PlayerInput>();
            opponent = gameManager.pvp.opponentGameManager;
        }

        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        gameManager.player.maxAmmo = 7;

        skillAction = playerInput.actions.FindAction("Skill");
        skillAction.performed += ctx => ActivateSkill();

        gameDisplay.costText.text = cost.ToString();
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            gameDisplay.SkillCooldownUpdate(cooldownTimer);

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
            }
            Debug.Log($"Remaining Time for skill: {cooldownTimer}");
        }
    }

    public void ActivateSkill()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            return;
        }
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }
        if (gameManager.isTimeStopped) return;

        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        StartCoroutine(TimeStopOpponent(12f));
    }

    public IEnumerator TimeStopOpponent(float durationSeconds)
    {
        opponent.isTimeStopped = true;
        yield return new WaitForSeconds(durationSeconds);
        opponent.isTimeStopped = false;

        Debug.Log("Time unfroze.");
    }

}
