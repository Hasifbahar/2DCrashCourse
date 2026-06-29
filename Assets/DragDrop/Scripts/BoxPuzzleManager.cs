using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MaskTransitions; // ADDED: We need this to talk to your TransitionManager!

public class BoxPuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;
    [SerializeField] private GameObject[] workers;
    [SerializeField] private Animator[] workerAnimator;
    [SerializeField] private RectTransform[] workerPositions;
    
    // ADDED: Scene name variables just like in your FightPuzzleManager!
    [SerializeField] private string loseScene = "AlternateLevel2";
    [SerializeField] private string winScene = "Level2";

    private Vector2[] startPositions;

    private void Start()
    {
        GetWorkerGO();
        winUI.SetActive(false);
        tryUI.SetActive(false);
        SetIdle(true);
        startPositions = new Vector2[workerPositions.Length];

        for (int i = 0; i < workerPositions.Length; i++)
        {
            startPositions[i] = workerPositions[i].anchoredPosition;
        }
    }

    private void GetWorkerGO ()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            workerAnimator[i] = workers[i].GetComponent<Animator>();
            workerPositions[i] = workers[i].GetComponent<RectTransform>();
        }
    }

    public void CheckAnswer()
    {
        StartCoroutine(CheckAndPlay());
    }
    
    IEnumerator CheckAndPlay()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);

        SetIdle(true);

        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsCorrect())
            {
                // Leave idle
                workerAnimator[i].SetBool("Idle", false);

                // Play push animation
                workerAnimator[i].SetTrigger("Push");
                workerPositions[i].DOKill();

                workerPositions[i]
                    .DOAnchorPosX(startPositions[i].x + 340, 0.5f)
                    .SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(1f);

                // Return to idle
                workerAnimator[i].SetBool("Idle", true);

                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                // Leave idle
                workerAnimator[i].SetBool("Idle", false);

                // Play die animation
                workerAnimator[i].SetTrigger("Die");

                yield return new WaitForSeconds(1.5f);

                tryUI.SetActive(true);
                
                // Move all workers back instantly
                for (int j = 0; j < workers.Length; j++)
                {
                    workerPositions[j].anchoredPosition = startPositions[j];
                }
                
                // ADDED: Wait for the UI to be seen, then load the lose scene!
                yield return new WaitForSeconds(2f);
                TransitionManager.Instance.LoadLevel(loseScene, 0.5f);
                
                yield break;
            }
        }

        // WIN
        yield return new WaitForSeconds(0.5f);

        winUI.SetActive(true);

        yield return new WaitForSeconds(2f);
        for (int j = 0; j < workers.Length; j++)
        {
            workerPositions[j].anchoredPosition = startPositions[j];
        }
        
        // FIXED: The comment slashes are gone, and it now uses your winScene variable!
        TransitionManager.Instance.LoadLevel(winScene, 0.5f);
    }

    void SetIdle(bool state)
    {
        for (int i = 0; i < workerAnimator.Length; i++)
        {
            workerAnimator[i].SetBool("Idle", state);
        }
    }
}