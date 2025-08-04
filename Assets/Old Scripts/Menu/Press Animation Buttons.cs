using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PressAnimationButtons : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText;
    public float pressOffset = 4f;

    private Vector3 originalPosition;

    void Start()
    {
        if (buttonText == null)
        {
            Debug.LogError("TextMeshProUGUI reference not set!");
            return;
        }

        originalPosition = buttonText.rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonText.rectTransform.localPosition = originalPosition - new Vector3(0, pressOffset, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonText.rectTransform.localPosition = originalPosition;
    }
}
