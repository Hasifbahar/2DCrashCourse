using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Iinteractable interactableInRange = null; //Closest Interactable
    public GameObject interactionIcon;

    // Start is called before the first frame update
    void Start()
    {
        interactionIcon.SetActive(false);
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            if (interactableInRange != null)
            {
                interactableInRange.Interact();
            }
            else
            {
                Debug.Log("Button pressed, but nothing in range!");
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Iinteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Iinteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }

    }

}
