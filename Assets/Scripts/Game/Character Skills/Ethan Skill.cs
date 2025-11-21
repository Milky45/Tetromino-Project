using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EthanSkill : MonoBehaviour
{
    [Header("References")]
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public CharacterManager characterManager;
    public Game_Manager opponent;
    public GameObject opponentMashDisplay;
    public MashBar oppMashBar;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 50f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Misc")]
    public bool isSec = false;
    public int cost = 500;
    public float tempGravity = 0f;

    private void Start()
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
        opponentMashDisplay = gameManager.pvp.opponentGameManager.gameDisplay.mashBarDisplay;
        oppMashBar = opponentMashDisplay.GetComponent<MashBar>();
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        gameManager.player.maxAmmo = 5;

        if (isSec == true)
        {
            Debug.Log("Ethan Skill Assigned as Secondary Skill");
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            Debug.Log("Ethan Skill Assigned as Primary Skill");
            skillAction = playerInput.actions.FindAction("Skill");
            gameDisplay.cost1Text.text = cost.ToString();
        }       
        if (skillAction != null)
           skillAction.performed += ctx => ActivateSkill();

    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            if(isSec)
            {
                gameDisplay.Skill2CooldownUpdate(cooldownTimer);
            }
            else
            {
                gameDisplay.Skill1CooldownUpdate(cooldownTimer);
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
            }
        }
    }

    public void ActivateSkill()
    {
        if (gameManager.isGameOver) return;
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
        gameManager.shaker.ChipsDeductShake();
        gameManager.pvp.opponentGameManager.gameDisplay.mashBarDisplay.SetActive(true);
        oppMashBar.enabled = true;
        oppMashBar.bar.value = 0f;
        oppMashBar.currentValue = 0f;
        oppMashBar.mashAnim.Play("Pop Out", 0, 0f);
        StartCoroutine(TimeStopOpponent(12f));
        gameManager.shaker.boardShake();
        opponent.shaker.boardShake();
        StartCoroutine(gameDisplay.BackPulse(12f, "#763700"));
    }

    public IEnumerator TimeStopOpponent(float durationSeconds)
    {
        gameManager.audioManager.PlaySFX(gameManager.audioManager.EthanSfx);
        tempGravity = gameManager.currentGravityDelay;

        gameManager.currentGravityDelay = 99f;
        gameManager.player.maxAmmo = 3;
        if (gameManager.player.attackAmmo > gameManager.player.maxAmmo)
        {
            gameManager.player.attackAmmo = gameManager.player.maxAmmo;
        }
        gameDisplay.Ammo_Update(gameManager.player.attackAmmo);
        opponent.isTimeStopped = true;

        yield return new WaitForSeconds(durationSeconds);

        gameManager.pvp.opponentGameManager.gameDisplay.mashBarDisplay.SetActive(false);
        oppMashBar.enabled = false;
        gameManager.currentGravityDelay = tempGravity;
        opponent.isTimeStopped = false;
        oppMashBar.mashAnim.SetTrigger("Minimize");
        oppMashBar.bar.value = 0f;
        gameManager.player.maxAmmo = 5;
        gameDisplay.Ammo_Update(gameManager.player.attackAmmo);

        Debug.Log("Time unfroze.");
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }

}
