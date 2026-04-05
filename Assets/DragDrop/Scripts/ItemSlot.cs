using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        if (eventData.pointerDrag != null)
        {
            RectTransform dragged = eventData.pointerDrag.GetComponent<RectTransform>();

            // Set as child of slot
            dragged.SetParent(transform);

            // Reset position inside slot
            dragged.anchoredPosition = Vector2.zero;
        }
    }

    public void ClearSlot()
    {
        // Remove all children (items) from the slot
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
