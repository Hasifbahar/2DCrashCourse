using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject tryUI;

    private void Start()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);
    }
    public void CheckAnswer()
    {
        winUI.SetActive(false);
        tryUI.SetActive(false);
        foreach (ItemSlot slot in slots)
        {
            if (!slot.IsCorrect())
            {
                WrongAnswer();
                return;
            }
        }

        CorrectAnswer();
    }

    void CorrectAnswer()
    {
        Debug.Log("Correct!");
        // Worker moves
        winUI.SetActive(true);
    }

    void WrongAnswer()
    {
        Debug.Log("Type mismatch detected.");
        // Worker freeze animation
        tryUI.SetActive(true);
    }
}