using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button btn;
    public float moveDistance = 5f; // distance

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = btn.GetComponent<RectTransform>().localPosition;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 newPosition = originalPosition;
        newPosition.y -= moveDistance;
        btn.GetComponent<RectTransform>().localPosition = newPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        btn.GetComponent<RectTransform>().localPosition = originalPosition;
    }
}
