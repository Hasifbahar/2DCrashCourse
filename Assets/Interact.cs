using MaskTransitions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interact : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string roomName = "Room A";
    [SerializeField] private PlayerController input;
    [SerializeField] private GameObject interactUI = null;
    [SerializeField] private bool isPlayerInside = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            if (interactUI != null) interactUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInside && input != null && input.interact) // we’ll add this
        {
            EnterRoom();
            input.interact = false; // reset input
        }
    }

    private void EnterRoom()
    {
        Debug.Log("Entering " + roomName);

        TransitionManager.Instance.LoadLevel(roomName); // Load the scene with the same name as the room
        interactUI.SetActive(false);
    }
}