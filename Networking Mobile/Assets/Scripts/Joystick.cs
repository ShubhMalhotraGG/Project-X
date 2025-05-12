using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform handle;
    private RectTransform background;
    private Vector2 inputVector;

    void Start()
    {
        background = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        localPoint /= background.sizeDelta;
        inputVector = new Vector2(localPoint.x * 2, localPoint.y * 2);
        inputVector = inputVector.magnitude > 1f ? inputVector.normalized : inputVector;

        handle.anchoredPosition = new Vector2(
            inputVector.x * (background.sizeDelta.x / 2),
            inputVector.y * (background.sizeDelta.y / 2));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Horizontal input from -1 to 1
    /// </summary>
    public float Horizontal => inputVector.x;

    /// <summary>
    /// Vertical input from -1 to 1
    /// </summary>
    public float Vertical => inputVector.y;
}