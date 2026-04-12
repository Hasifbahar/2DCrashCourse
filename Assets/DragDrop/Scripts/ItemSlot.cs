using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string correctType; // expected answer

    private DraggableItemData currentItem;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ClearSlot();

            RectTransform dragged = eventData.pointerDrag.GetComponent<RectTransform>();
            dragged.SetParent(transform);
            dragged.anchoredPosition = Vector2.zero;

            currentItem = dragged.GetComponent<DraggableItemData>();
        }
    }

    public bool IsCorrect()
    {
        if (currentItem == null) return false;
        return currentItem.typeName == correctType;
    }

    public void ClearSlot()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        currentItem = null;
    }
}