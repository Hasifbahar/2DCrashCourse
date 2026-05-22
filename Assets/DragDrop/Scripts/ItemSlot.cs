using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public string correctType; // expected answer
    [SerializeField] private bool isCorrectAnswer; // whether the slot has the correct item
    private DraggableItemData currentItem;

    private void Start()
    {
        currentItem = GetComponentInChildren<DraggableItemData>();
        if (currentItem != null)
        {
            isCorrectAnswer = currentItem.typeName == correctType;
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            ClearSlot();

            RectTransform dragged = eventData.pointerDrag.GetComponent<RectTransform>();
            dragged.SetParent(transform, false);
            dragged.localPosition = Vector3.zero;
            currentItem = dragged.GetComponent<DraggableItemData>();
            if (currentItem != null)
            {
                isCorrectAnswer = currentItem.typeName == correctType;
            }
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