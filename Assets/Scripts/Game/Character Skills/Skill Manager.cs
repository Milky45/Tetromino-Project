using UnityEngine;
using UnityEngine.InputSystem;

public class SkillManager : MonoBehaviour
{
    [Header("References")]
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public AudioManager audioManager;
    public CharSkills charSkills;
    public Collider2D characterCollider;
    public Board_Manager boardManager;

    [Header("Cooldown UI")]
    public GameDisplay gameDisplay;
    private Coroutine pulseRoutine;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Misc")]
    public bool isPlayer1 = true;
    public bool isChar1 = true;
    public int burnCtr = 0;

    [Header("Cooldown Settings")]
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private float cooldownTime = 0f;

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            if(isChar1)
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

    private void ActivateSkill()
    {
        if (!isOnCooldown && gameManager.player.currentChips >= charSkills.skillCost)
        {
            Debug.Log("Skill activated!");
            
            switch (charSkills.characterID)
            {
                case 1:
                    // Activate skill for Character 1
                    break;
                case 2:
                    // Activate skill for Character 2
                    break;
                // Add more cases as needed
            }

            isOnCooldown = true;
            cooldownTimer = cooldownTime;
            PlaySkillSound();
        }
        else
        {
            Debug.Log("Skill is on cooldown. Please wait.");
        }
    }

    private void PlaySkillSound()
    {

    }
}
