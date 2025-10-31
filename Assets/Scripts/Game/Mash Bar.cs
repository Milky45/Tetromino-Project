using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MashBar : MonoBehaviour
{
    [Header("Bar Settings")]
    public Slider bar;
    public KeyCode mashKey = KeyCode.Space;
    public float increasePerPress = 0.05f;
    public float decayRate = 0.05f; // units per second
    public float maxValue = 1f;

    public PlayerInput playerInput;
    public InputAction mashAction;

    private float currentValue = 0f;

    void Awake()
    {
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
    }

    public void IncValue()
    {
        currentValue += increasePerPress;
    }
}
