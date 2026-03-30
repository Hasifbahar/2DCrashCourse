using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // REQUIRED for Image
using TMPro;          // REQUIRED for Text

public class Board : MonoBehaviour, Iinteractable
{
    public bool IsOpened { get; private set; }
    public string BoardID { get; private set; }

    [Header("Drops")]
    public GameObject itemPrefab;

    [Header("UI Visuals")]
    // The Sprite to change this object to when opened
    public Sprite openedSprite; 
    
    // The separate UI panel that contains the text (The "Popup")
    public GameObject textPopupPanel; 
    // The text component inside the popup
    public TextMeshProUGUI messageText; 
    // The actual message to write
    [TextArea] public string contentText = "Write your message here...";

    void Start()
    {
        BoardID ??= GlobalHelper.GenerateUniqueID(gameObject);
        
        // Ensure popup is hidden at start
        if(textPopupPanel != null)
            textPopupPanel.SetActive(false);
    }

    public bool CanInteract()
    {
        // Allows interaction if not open, OR allows reading it again if you prefer
        return true; 
    }

    public void Interact()
    {
        // If it's the first time opening
        if (!IsOpened)
        {
            OpenBoard();
        }
        else
        {
            // If already open, just toggle the text reading panel
            ToggleTextPopup();
        }
    }

    private void OpenBoard()
    {
        IsOpened = true;

        // 1. Change the SELF image from Closed to Open
        // We use TryGetComponent in case you forgot to add the Image component
        if(TryGetComponent<Image>(out Image boardImage))
        {
            boardImage.sprite = openedSprite;
        }

        // 2. Show the Text Popup
        ToggleTextPopup();

        // 3. Drop Item (if any)
        if (itemPrefab)
        {
            Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
        }
    }

    private void ToggleTextPopup()
    {
        if (textPopupPanel != null)
        {
            bool isActive = textPopupPanel.activeSelf;
            textPopupPanel.SetActive(!isActive);

            // Set the text when opening
            if (!isActive && messageText != null)
            {
                messageText.text = contentText;
            }
        }
    }
}