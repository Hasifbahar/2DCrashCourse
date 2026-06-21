using MaskTransitions;
using UnityEngine;

// Notice there is no ", Iinteractable" here! This script works completely on its own.
public class LevelTransitionDoor : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private string roomName = "Room A";
    
    [Tooltip("If false, the player automatically teleports on touch. If true, they must press E.")]
    [SerializeField] private bool requireButtonPress = true;

    [Header("UI (Optional)")]
    [SerializeField] private GameObject interactUI = null;

    private bool isPlayerInside = false;

    private void Start()
    {
        // Ensure the UI is hidden when the game starts
        if (interactUI != null) interactUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!requireButtonPress)
            {
                // Teleport immediately if button press isn't required
                EnterRoom();
            }
            else
            {
                // Wait for the button press and show the UI
                isPlayerInside = true;
                if (interactUI != null) interactUI.SetActive(true);
            }
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
        // Safely check for the E key only when the player is inside the trigger zone
        if (isPlayerInside && requireButtonPress && Input.GetKeyDown(KeyCode.E))
        {
            EnterRoom();
        }
    }

    private void EnterRoom()
    {
        // Safety check to prevent triggering multiple times
        isPlayerInside = false; 
        if (interactUI != null) interactUI.SetActive(false);

        Debug.Log("Entering " + roomName);
        TransitionManager.Instance.LoadLevel(roomName); 
    }
}