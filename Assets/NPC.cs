using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;          

public class NPC : MonoBehaviour, Iinteractable
{
	public NPC_Dialogue dialogueData;
	public GameObject dialoguePanel; 
	public TMP_Text dialogueText, nameText;
	public Image portraitImage;

	private int dialogueIndex;
	private bool isTyping, isDialogueActive;

	public bool CanInteract()
	{
		return !isDialogueActive;
	}

	public void Interact()
	{
        // Temporarily removed PauseController check so you can test the game
		if (dialogueData == null) 
			return;

		if (isDialogueActive)
		{
			NextLine();
		}
		else
		{
			StartDialogue();
		}
	}

	void StartDialogue()
	{
		isDialogueActive = true; 
		dialogueIndex = 0;

		nameText.SetText(dialogueData.npcName); 
		portraitImage.sprite = dialogueData.npcPortrait;

		dialoguePanel.SetActive(true);
        
        // PauseController.SetPause(true); // Commented out until you create this script

		StartCoroutine(TypeLine());
	}

	void NextLine()
	{
		if (isTyping)
		{
			StopAllCoroutines();
			dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
			isTyping = false;
		}
		else if(++dialogueIndex < dialogueData.dialogueLines.Length)
		{
			StartCoroutine(TypeLine());
		}
		else
		{
			EndDialogue();
		}
	}

	IEnumerator TypeLine()
	{
		isTyping = true;
		dialogueText.SetText("");

		foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
		{
			dialogueText.text += letter;
			yield return new WaitForSeconds(dialogueData.typingSpeed);
		}

		isTyping = false;

        // FIXED: Changed to lowercase 'l' to match your NPC_Dialogue script exactly
		if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
		{
			yield return new WaitForSeconds(dialogueData.autoProgressDelay);
			NextLine();
		}
	}

	public void EndDialogue()
	{
		StopAllCoroutines();
		isDialogueActive = false;
		dialogueText.SetText("");
		dialoguePanel.SetActive(false);
        
        // PauseController.SetPause(false); // Commented out until you create this script
	}
}