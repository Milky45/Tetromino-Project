using UnityEngine;

public class TriggerSkills : MonoBehaviour
{
    [Header("References")]
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public AudioManager audioManager;
    public CharSkills charSkills;
    public Collider2D characterCollider;
    public Board_Manager boardManager;
    public SkillCoroutines skillCoroutines;
    public SkillManager skillManager;
    public RockGroupHandler rockGroupHandler;

    public void TetroSkill() // NOTE: this function will be called in the animation event of the tetro skill animation 
    {
        if (skillCoroutines.pulseRoutine != null)
            StopCoroutine(skillCoroutines.pulseRoutine);
        skillCoroutines.pulseRoutine = StartCoroutine(skillCoroutines.BlindPulse());
    }

    public void ScorchSkill() // NOTE: this is not finished yet. will still clean up the board and active piece logic
    {
        //ClearBottomLines();
        gameManager.shaker.boardShake();
        StartCoroutine(gameManager.gameDisplay.BackPulse(8f, $"{skillManager.skillColor}"));
        //StartCoroutine(gameManager.gameDisplay.BackPulse(8f, "#00763bff"));
    }

    public void PackHatSkill()
    {
        skillManager.characterSkillAnim.SetTrigger("Activate");
        skillCoroutines.EnableZeroAttackCooldownForTenSeconds();
        StartCoroutine(gameManager.gameDisplay.BackPulse(10f, $"{skillManager.skillColor}"));
        //StartCoroutine(gameManager.gameDisplay.BackPulse(10f, "#bb6400ff"));
    }

    public void DodokeSkill()
    {
        StartCoroutine(skillCoroutines.BoardFlip());
        gameManager.pvp.opponentGameManager.shaker.boardShake();
        StartCoroutine(gameManager.pvp.opponentGameManager.gameDisplay.BackPulse(10f, $"{skillManager.skillColor}"));
        //StartCoroutine(gameManager.pvp.opponentGameManager.gameDisplay.BackPulse(10f, "#720076ff"));
    }

    public void YunJinSkill()
    {
        switch (skillManager.skillPhase)
        {
            case 0:
                skillManager.skillPhase = 1; // Move to the next phase
                // summon up to 3 rocks
                rockGroupHandler.SpawnRocks();
                break;
            case 1:
                // push rocks forward
                rockGroupHandler.PushRockGroup();
                break;
            default:
                break;
        }
        skillManager.characterSkillAnim.SetTrigger("Activate");
    }
}
