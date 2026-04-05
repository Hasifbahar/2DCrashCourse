using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    [SerializeField] private Canvas canvas;
    [SerializeField] private bool isTemplate = false;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");

        if (isTemplate)
        {
            // Create clone to drag
            GameObject clone = Instantiate(gameObject, transform.parent);
            clone.GetComponent<DragDrop>().isTemplate = false;

            // Forward drag to clone
            eventData.pointerDrag = clone;
            ExecuteEvents.Execute(clone, eventData, ExecuteEvents.beginDragHandler);

            return;
        }

        originalParent = transform.parent;

        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Not dropped into a slot > delete
        if (transform.parent == canvas.transform)
        {
            Destroy(gameObject);
        }
    }
}
