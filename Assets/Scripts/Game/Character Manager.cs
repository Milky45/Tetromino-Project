using UnityEditor.Animations;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public bool isPlayer1 = true;
    private Animator animator;
    private CharacterSelect playerSelect;
    public AnimatorController[] charControllers = new AnimatorController[8];
    public GameObject blindBall;
    public GameObject blindOverlay;
    public Animator animBall;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
            return;
        }
        if (isPlayer1)
        {
            playerSelect = GameObject.Find("Player 1").GetComponent<CharacterSelect>();
            Debug.Log("CharacterManager assigned to Player 1");
        }
        else
        {
            playerSelect = GameObject.Find("Player 2").GetComponent<CharacterSelect>();
            Debug.Log("CharacterManager assigned to Player 2");
        }

        SetCharacter(playerSelect.selectedCharacterIndex);
    }

    public void SetCharacter(int characterIndex)
    {
        if (characterIndex == 7)
        {
            //randomly select a character controller to assign in animator
            characterIndex = Random.Range(0, charControllers.Length);
            Debug.Log($"Randomly selected character index: {characterIndex}");
        }

        // assign tha animator controller to the animator

        animator.runtimeAnimatorController = charControllers[characterIndex];
        Debug.Log($"Character set to index: {characterIndex}");
        // Update the character display in the CharacterSelect script
        SetCharacterSkillScript(characterIndex);
    }

    public void SetCharacterSkillScript(int characterIndex)
    {
        switch (characterIndex)
        {
            case 0:
                gameObject.AddComponent<TetroSkill>();
                Debug.Log("TetroSkill script assigned for character index 0");
                break;
            case 1:
                gameObject.AddComponent<PackHatSkill>();
                Debug.Log("PackhatSkill script assigned for character index 1");
                break;
            case 2:
                gameObject.AddComponent<ScorchSkill>();
                Debug.Log("ScorchSkill script assigned for character index 2");
                break;
            case 3:
                gameObject.AddComponent<DodokeSkill>();
                Debug.Log("DodokeSkill script assigned for character index 3");
                break;
            case 4:
                gameObject.AddComponent<YunJinSkill>();
                Debug.Log("YunJInSkill script assigned for character index 3");
                break;
            case 5:
                gameObject.AddComponent<NullSkill>();
                Debug.Log("NullSkill script assigned for character index 5");
                break;
            case 6:
                gameObject.AddComponent<EthanSkill>();
                Debug.Log("EthanSkill script assigned for character index 5");
                break;
            default:
                Debug.LogWarning($"No skill script assigned for character index: {characterIndex}");
                break;
        }
    }
}
