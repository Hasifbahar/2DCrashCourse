using System.Collections;
using UnityEngine;

public class ShootPuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;
    [SerializeField] private Animator workerAnimator;

    private void Start()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);

        // Start by triggering the ShootIdle animation
        TriggerAnimation("ShootIdle");
    }

    // Call this function when the player clicks the check button
    public void CheckAnswer()
    {
        // 1. Stop any running sequences to prevent overlapping
        StopAllCoroutines();

        // 2. Create a flag to track if the overall answer is correct
        bool isAllCorrect = true;

        // 3. Check every slot. If ANY slot is wrong, the player hasn't won yet.
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsCorrect())
            {
                isAllCorrect = false;
                break; // We found a wrong answer! No need to check the rest.
            }
        }

        // 4. Fire the correct sequence exactly ONCE based on the final result
        if (isAllCorrect)
        {
            StartCoroutine(PlayWinSequence());
        }
        else
        {
            StartCoroutine(PlayLoseSequence());
        }
    }

    private IEnumerator PlayWinSequence()
    {
        TriggerAnimation("ShootHit1");

        // Wait for the duration of the Hit1 animation (e.g., 1.5 seconds)
        yield return new WaitForSeconds(1.5f);

        TriggerAnimation("ShootWin");
        winUI.SetActive(true);
    }

    private IEnumerator PlayLoseSequence()
    {
        // 1. Play the Hit2 animation
        TriggerAnimation("ShootHit2");

        // Wait for the duration of the Hit2 animation (e.g., 1.5 seconds)
        yield return new WaitForSeconds(1.5f);

        // 2. Transition to the looping Lose animation and show UI
        TriggerAnimation("ShootLose");
        tryUI.SetActive(true);

        // 3. Let it loop for exactly 3 seconds
        yield return new WaitForSeconds(3.0f);

        // 4. Clean up: Hide the try UI and loop back to ShootIdle
        tryUI.SetActive(false);
        TriggerAnimation("ShootIdle");
    }

    // Helper method to cleanly reset triggers and set the new animation
    private void TriggerAnimation(string triggerName)
    {
        workerAnimator.ResetTrigger("ShootIdle");
        workerAnimator.ResetTrigger("ShootHit1");
        workerAnimator.ResetTrigger("ShootHit2");
        workerAnimator.ResetTrigger("ShootWin");
        workerAnimator.ResetTrigger("ShootLose");

        workerAnimator.SetTrigger(triggerName);
    }
}