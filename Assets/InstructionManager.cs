using UnityEngine;
using UnityEngine.InputSystem; // We need this for the 'Q' key!

public class InstructionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag your Instruction Panel here!")]
    public GameObject instructionPanel;

    void Start()
    {
        // 1. Pop up straight away when the level loads!
        if (instructionPanel != null)
        {
            instructionPanel.SetActive(true);
        }
    }

    void Update()
    {
        // 2. Only check for the 'Q' key if the panel is currently visible
        if (instructionPanel != null && instructionPanel.activeSelf)
        {
            // 3. If the player presses 'Q' on their keyboard...
            if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
            {
                // ...Hide the panel!
                instructionPanel.SetActive(false);
            }
        }
    }
}