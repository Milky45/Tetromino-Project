using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashBar : MonoBehaviour
{
    [Header("Bar Settings")]
    public Slider bar;
    public float increasePerPress = 0.05f;
    public float decayRate = 0.05f; // units per second
    public float maxValue = 1f;
    public Animator mashAnim;
    public PlayerInput playerInput;
    public InputAction mashAction;
    public Game_Manager gameManager;
    public GameObject characterScript;
    public EthanSkill oppEthanSkill;
    public float currentValue = 0f;

    private void Start()
    {
        playerInput = gameManager.playerInput;
        oppEthanSkill = characterScript.GetComponent<EthanSkill>();
        mashAction = playerInput.actions.FindAction("Hard Drop");
        mashAction.performed += ctx => IncValue();
    }

    void Update()
    {
        // Decay over time
        currentValue -= decayRate * Time.deltaTime;

        // Clamp to 0–max range
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);

        // Update UI
        if (bar != null)
            bar.value = currentValue;
        if (bar.value >= maxValue)
        {
            EscapeTimeStop();
        }
    }

    public void EscapeTimeStop() // no Romel Pun intended // Bring back current stats of all players
    {
        oppEthanSkill.gameManager.currentGravityDelay = oppEthanSkill.tempGravity;
        oppEthanSkill.gameManager.pvp.opponentGameManager.isTimeStopped = false;
        mashAnim.SetTrigger("Minimize");
        oppEthanSkill.oppMashBar.enabled = false;
        bar.value = 0f;
        currentValue = 0f;
        Invoke(nameof(HideDisplayEscaped), 1f);
    }

    public void HideDisplayEscaped()
    {
        gameManager.gameDisplay.mashBarDisplay.SetActive(false);
    }

    public void IncValue()
    {
        string playerID = gameManager.player.isPlayer1 ? "P1" : "P2";
        Debug.Log($"{playerID} is hitting mashing!");
        currentValue += increasePerPress;
    }

    private void OnDisable()
    {
        mashAction.performed -= ctx => IncValue();
    }
}
