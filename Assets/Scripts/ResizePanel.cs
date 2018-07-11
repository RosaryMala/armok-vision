using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizePanel : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    public Vector2 minSize = new Vector2(100, 100);
    public Vector2 maxSize = new Vector2(400, 400);
    public bool lockWidth;
    public bool lockHeight;
    public bool reverseX;
    public bool reverseY;

    private RectTransform panelRectTransform;
    private Vector2 originalLocalPointerPosition;
    private Vector2 originalSizeDelta;

    void Awake()
    {
        panelRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        originalSizeDelta = panelRectTransform.sizeDelta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out originalLocalPointerPosition);
    }

    public void OnDrag(PointerEventData data)
    {
        if (panelRectTransform == null)
            return;

        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out localPointerPosition);
        Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;

        Vector2 sizeDelta = originalSizeDelta + new Vector2((lockWidth ? 0 : offsetToOriginal.x) * (reverseX ? -1 : 1), (lockHeight ? 0 : -offsetToOriginal.y) * (reverseY ? -1 : 1));
        sizeDelta = new Vector2(
            Mathf.Clamp(sizeDelta.x, minSize.x, maxSize.x),
            Mathf.Clamp(sizeDelta.y, minSize.y, maxSize.y)
        );

        panelRectTransform.sizeDelta = sizeDelta;
    }
}