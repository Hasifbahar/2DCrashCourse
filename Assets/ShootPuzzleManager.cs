using System.Collections;
using UnityEngine;
using MaskTransitions; 

public class ShootPuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;
    [SerializeField] private Animator workerAnimator;

    [SerializeField] private string winScene = "Level4"; 
    [SerializeField] private string loseScene = "AlternateLevel4"; 

    private void Start()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);

        // Start by triggering the ShootIdle animation
        TriggerAnimation("ShootIdle");
    }

    public void CheckAnswer()
    {
        StopAllCoroutines();

        bool isAllCorrect = true;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsCorrect())
            {
                isAllCorrect = false;
                break; 
            }
        }

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
        // 1. Play the shooting and dodging action
        TriggerAnimation("ShootHit1");

        // IMPORTANT: Change '1.0f' to the exact length of your ShootHit1 clip in seconds!
        yield return new WaitForSeconds(1.0f);

        // 2. Transition into the final Win loop and show UI
        TriggerAnimation("ShootWin");
        winUI.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        TransitionManager.Instance.LoadLevel(winScene, 0.5f);
    }

    private IEnumerator PlayLoseSequence()
    {
        // 1. Play the getting hit action
        TriggerAnimation("ShootHit2");

        // IMPORTANT: Change '1.0f' to the exact length of your ShootHit2 clip in seconds!
        yield return new WaitForSeconds(1.0f);

        // 2. Transition into the final Lose loop and show UI
        TriggerAnimation("ShootLose");
        tryUI.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        tryUI.SetActive(false);
        TransitionManager.Instance.LoadLevel(loseScene, 0.5f);
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