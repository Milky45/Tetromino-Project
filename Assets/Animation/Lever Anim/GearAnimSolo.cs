using UnityEngine;

public class GearAnimSolo : MonoBehaviour
{
    private Animator animator;
    private P1Controller controls;
    public static int ShiftInterval = P1Settings.GetShiftInterval();
    public static float currentSens = P1Settings.GetSensitivity();
    private int currentGear = 0; // -2 to 3
    private readonly float[] moveDelays = { currentSens, currentSens - (0.01f * ShiftInterval), currentSens - (0.02f * ShiftInterval), currentSens - (0.03f * ShiftInterval), currentSens + (0.01f * ShiftInterval), currentSens + (0.02f * ShiftInterval) };

    void Awake()
    {
        animator = GetComponent<Animator>();
        controls = new P1Controller();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.SensShiftSolo.NeutralRS.performed += ctx => SetGear(0);
        controls.SensShiftSolo.Neutral.performed += ctx => TriggerState("Neutral");
        controls.SensShiftSolo.Gear1.performed += ctx => TriggerState("G1");
        controls.SensShiftSolo.Gear2.performed += ctx => TriggerState("G2");
        controls.SensShiftSolo.Gear3.performed += ctx => TriggerState("G3");
        controls.SensShiftSolo.GearR1.performed += ctx => TriggerState("G -1");
        controls.SensShiftSolo.GearR2.performed += ctx => TriggerState("G -2");
        controls.SensShiftSolo.SwitchUpGear.performed += ctx => ChangeGear(-1);
        controls.SensShiftSolo.SwitchDownGear.performed += ctx => ChangeGear(+1);
    }

    void OnDisable()
    {
        controls.SensShiftSolo.NeutralRS.performed -= ctx => SetGear(0);
        controls.SensShiftSolo.Neutral.performed -= ctx => TriggerState("Neutral");
        controls.SensShiftSolo.Gear1.performed -= ctx => TriggerState("G1");
        controls.SensShiftSolo.Gear2.performed -= ctx => TriggerState("G2");
        controls.SensShiftSolo.Gear3.performed -= ctx => TriggerState("G3");
        controls.SensShiftSolo.GearR1.performed -= ctx => TriggerState("G -1");
        controls.SensShiftSolo.GearR2.performed -= ctx => TriggerState("G -2");
        controls.SensShiftSolo.SwitchUpGear.performed -= ctx => ChangeGear(-1);
        controls.SensShiftSolo.SwitchDownGear.performed -= ctx => ChangeGear(+1);

        controls.Disable();
    }

    void TriggerState(string triggerName)
    {
        ResetAllTriggers();
        switch (triggerName)
        {
            case "Neutral":
                GameManagerSolo.movementSensitivity = moveDelays[0];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[0]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
            case "G1":
                GameManagerSolo.movementSensitivity = moveDelays[1];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[1]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
            case "G2":
                GameManagerSolo.movementSensitivity = moveDelays[2];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[2]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
            case "G3":
                GameManagerSolo.movementSensitivity = moveDelays[3];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[3]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
            case "G -1":
                GameManagerSolo.movementSensitivity = moveDelays[4];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[4]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
            case "G -2":
                GameManagerSolo.movementSensitivity = moveDelays[5];
                Debug.Log($"P1 Triggered: {triggerName}, Move Delay:{moveDelays[5]}, in game: {GameManagerSolo.movementSensitivity}");
                break;
        }
        animator.SetTrigger(triggerName);
    }

    void ResetAllTriggers()
    {
        string[] all = { "Neutral", "G1", "G2", "G3", "G -1", "G -2" };
        foreach (var t in all) animator.ResetTrigger(t);
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
}
