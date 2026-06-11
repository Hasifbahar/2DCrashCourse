using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SniperPuzzleManager : MonoBehaviour
{
    [Header("UI and Slots")]
    public ItemSlot[] slots; // Needs exactly 3 slots!
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;

    [Header("Animation")]
    [Tooltip("Drag the parent object that holds your single Animator here")]
    [SerializeField] private Animator mainAnimator;

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
        yield return new WaitForSeconds(0.2f);

        // Check the 3 drop slots
        bool isModifierCorrect = slots[0].IsCorrect(); // Did they put 'private'?
        bool isTypeCorrect = slots[1].IsCorrect();     // Did they put 'boolean'?
        bool isValueCorrect = slots[2].IsCorrect();    // Did they put 'false'?

        // OUTCOME 1: SYNTAX ERROR (All Idle)
        if (!isTypeCorrect || !isValueCorrect)
        {
            Debug.Log("Syntax Error: Data type or value is wrong.");
            yield return new WaitForSeconds(1f);
            
            tryUI.SetActive(true);
            yield break;
        }

        // OUTCOME 2: BOTH SHOT / LOSE
        if (!isModifierCorrect && isTypeCorrect && isValueCorrect)
        {
            Debug.Log("Access Error: Middle Character was left public!");
            SetIdle(false);
            
            // Play your existing "Both Shot" animation directly!
            if (mainAnimator != null) mainAnimator.Play("ShootHit2");
            
            yield return new WaitForSeconds(1.8f);

            tryUI.SetActive(true);

            // Reset back to idle
            SetIdle(true);
            yield break;
        }

        // OUTCOME 3: ONLY BACK SHOT / WIN!
        if (isModifierCorrect && isTypeCorrect && isValueCorrect)
        {
            Debug.Log("Encapsulation Success: Middle Character protected!");
            SetIdle(false);

            // Play your existing "Only Back Shot" animation directly!
            if (mainAnimator != null) mainAnimator.Play("ShootHit1");
            
            yield return new WaitForSeconds(1.8f);

            // WIN!
            winUI.SetActive(true);

            yield return new WaitForSeconds(2f);
            
            // Optional transition here:
            // TransitionManager.Instance.LoadLevel("LevelScene", 0.5f);
        }
    }

    void SetIdle(bool state)
    {
        // This forces the Animator to instantly jump back to your Idle cutscene
        if (mainAnimator != null && state == true)
        {
            mainAnimator.Play("ShootIdle"); 
        }
    }
}
