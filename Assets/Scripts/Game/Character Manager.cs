using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public bool isPlayer1 = true;
    public bool isSolo = false;
    private Animator primAnimator;
    private Animator secAnimator;
    private CharacterSelect playerSelect;
    public RuntimeAnimatorController[] charControllers = new RuntimeAnimatorController[9];
    public GameObject blindBall;
    public GameObject blindOverlay;
    public GameObject charSecDisplay;

    private void Awake()
    {
        primAnimator = GetComponent<Animator>();
        if (primAnimator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
            return;
        }
        if (isPlayer1)
        {
            playerSelect = GameObject.Find("Player 1").GetComponent<CharacterSelect>();
            Debug.Log("CharacterManager assigned to Player 1");
        }
        else if (!isPlayer1 && !isSolo)
        {
            playerSelect = GameObject.Find("Player 2").GetComponent<CharacterSelect>();
            Debug.Log("CharacterManager assigned to Player 2");
        }
        SetCharacter(playerSelect.primCharIndex, false);
        if (playerSelect.isTeamBattle && !isSolo)
        {
            secAnimator = charSecDisplay.GetComponent<Animator>();
            SetCharacter(playerSelect.secCharIndex, true);
        }
    }
    public void SetCharacter(int characterIndex, bool isSec)
    {
        if (characterIndex == 7)
        {
            //randomly select a character controller to assign in animator
            while (characterIndex == 7)
            {
                characterIndex = Random.Range(0, charControllers.Length);
            }
            
            Debug.Log($"Randomly selected character index: {characterIndex}");
        }

        // assign tha animator controller to the animator
        if(isSec)
        {
            secAnimator.runtimeAnimatorController = charControllers[characterIndex];
        }
        else
        {
            primAnimator.runtimeAnimatorController = charControllers[characterIndex];
        }
        Debug.Log($"Character set to index: {characterIndex}");
        // Update the character display in the CharacterSelect script
        if (!isSolo)
        {
            SetCharacterSkillScript(characterIndex, isSec); // primary char
        }
        
    }

    public void SetCharacterSkillScript(int characterIndex, bool isSec)
    {
        switch (characterIndex)
        {
            case 0:
                var tetroSkill = gameObject.AddComponent<TetroSkill>();
                if (isSec) tetroSkill.isSec = true;
                Debug.Log("TetroSkill script assigned for character index 0");
                break;
            case 1:
                var packHatSkill = gameObject.AddComponent<PackHatSkill>();
                if (isSec) packHatSkill.isSec = true;
                Debug.Log("PackhatSkill script assigned for character index 1");
                break;
            case 2:
                var scorchSkill = gameObject.AddComponent<ScorchSkill>();
                if (isSec) scorchSkill.isSec = true;
                Debug.Log("ScorchSkill script assigned for character index 2");
                break;
            case 3:
                var dodokeSkill = gameObject.AddComponent<DodokeSkill>();
                if (isSec) dodokeSkill.isSec = true;
                Debug.Log("DodokeSkill script assigned for character index 3");
                break;
            case 4:
                var yunJinSkill = gameObject.AddComponent<YunJinSkill>();
                if (isSec) yunJinSkill.isSec = true;
                Debug.Log("YunJInSkill script assigned for character index 4");
                break;
            case 5:
                var nullSkill = gameObject.AddComponent<NullSkill>();
                if (isSec) nullSkill.isSec = true;
                Debug.Log("NullSkill script assigned for character index 5");
                break;
            case 6:
                var ethanSkill = gameObject.AddComponent<EthanSkill>();
                if (isSec) ethanSkill.isSec = true;
                Debug.Log("EthanSkill script assigned for character index 6");
                break;
            case 8:
                var dScorchSkill = gameObject.AddComponent<D_ScorchSkill>();
                if (isSec) dScorchSkill.isSec = true;
                Debug.Log("ScorchSkill script assigned for character index 8");
                break;
            default:
                Debug.LogWarning($"No skill script assigned for character index: {characterIndex}");
                break;
        }
    }
}
