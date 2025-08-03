using UnityEngine;
using UnityEngine.InputSystem;

public class GearAnimatorP2 : MonoBehaviour
{
    private Animator animator;
    private P2Controller controls;
    public static int ShiftInterval = P2Settings.GetShiftInterval();
    public static float currentSens = P2Settings.GetSensitivity();
    private int currentGear = 0; // -2 to 3
    private readonly float[] moveDelays = { currentSens, currentSens - (0.01f * ShiftInterval), currentSens - (0.02f * ShiftInterval), currentSens - (0.03f * ShiftInterval), currentSens + (0.01f * ShiftInterval), currentSens + (0.02f * ShiftInterval) };


    void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new P2Controller();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.SensShift.Neutral.performed += ctx => SetGear(0);
        controls.SensShift.SwitchUpGear.performed += ctx => ChangeGear(-1);
        controls.SensShift.SwitchDownGear.performed += ctx => ChangeGear(+1);
    }

    void OnDisable()
    {
        controls.SensShift.Neutral.performed -= ctx => SetGear(0);
        controls.SensShift.SwitchUpGear.performed -= ctx => ChangeGear(-1);
        controls.SensShift.SwitchDownGear.performed -= ctx => ChangeGear(+1);
        controls.Disable();
    }

    void SetGear(int gear)
    {
        currentGear = Mathf.Clamp(gear, -2, 3);
        TriggerState(GearToTrigger(currentGear));
    }

    void ChangeGear(int delta)
    {
        currentGear = Mathf.Clamp(currentGear + delta, -2, 3);
        TriggerState(GearToTrigger(currentGear));
    }

    string GearToTrigger(int gear)
    {
        return gear switch
        {
            -2 => "G -2",
            -1 => "G -1",
            0 => "Neutral",
            1 => "G1",
            2 => "G2",
            3 => "G3",
            _ => "Neutral"
        };
    }

    void TriggerState(string triggerName)
    {
        ResetAllTriggers();
        switch (triggerName)
        {
            case "Neutral":
                GameManagerP2.movementSensitivity = moveDelays[0];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[0]}, in game: {GameManagerP2.movementSensitivity}");
                break;
            case "G1":
                GameManagerP2.movementSensitivity = moveDelays[1];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[1]}, in game: {GameManagerP2.movementSensitivity}");
                break;
            case "G2":
                GameManagerP2.movementSensitivity = moveDelays[2];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[2]}, in game: {GameManagerP2.movementSensitivity}");
                break;
            case "G3":
                GameManagerP2.movementSensitivity = moveDelays[3];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[3]}, in game: {GameManagerP2.movementSensitivity}");
                break;
            case "G -1":
                GameManagerP2.movementSensitivity = moveDelays[4];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[4]}, in game: {GameManagerP2.movementSensitivity}");
                break;
            case "G -2":
                GameManagerP2.movementSensitivity = moveDelays[5];
                Debug.Log($"P2 Triggered: {triggerName}, Move Delay:{moveDelays[5]}, in game: {GameManagerP2.movementSensitivity}");
                break;
        }
        animator.SetTrigger(triggerName);
        Debug.Log("P2 Triggered: " + triggerName);
    }

    void ResetAllTriggers()
    {
        string[] all = { "Neutral", "G1", "G2", "G3", "G -1", "G -2" };
        foreach (var t in all) animator.ResetTrigger(t);
    }
}
