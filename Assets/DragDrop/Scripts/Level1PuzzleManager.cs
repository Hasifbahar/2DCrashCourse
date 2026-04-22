using System.Collections;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;
    [SerializeField] private Animator workerAnimator;

    private void Start()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);

        SetIdle(true);
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
        yield return new WaitForSeconds(0.2f); // small pause before start

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsCorrect())
            {
                // Leave idle  play answer animation
                SetIdle(false);
                ResetAll();

                PlayCorrectAnimation(i);
                yield return new WaitForSeconds(0.8f);

                // Back to idle briefly
                ResetAll();
                SetIdle(true);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                SetIdle(false);
                ResetAll();

                workerAnimator.SetBool("dead", true);
                yield return new WaitForSeconds(3f);

                tryUI.SetActive(true);

                ResetAll();
                SetIdle(true);
                yield break;
            }
        }

        // All correct  win
        SetIdle(false);
        ResetAll();

        workerAnimator.SetBool("win", true);
        yield return new WaitForSeconds(1f);

        winUI.SetActive(true);

        ResetAll();
        SetIdle(true);
    }

    void PlayCorrectAnimation(int index)
    {
        switch (index)
        {
            case 0:
                workerAnimator.SetBool("ans1", true);
                break;
            case 1:
                workerAnimator.SetBool("ans2", true);
                break;
            case 2:
                workerAnimator.SetBool("ans3", true);
                break;
        }
    }
    void ResetAll()
    {
        workerAnimator.SetBool("ans1", false);
        workerAnimator.SetBool("ans2", false);
        workerAnimator.SetBool("ans3", false);
        workerAnimator.SetBool("dead", false);
        workerAnimator.SetBool("win", false);
    }

    void SetIdle(bool state)
    {
        workerAnimator.SetBool("Idle", state);
    }
}