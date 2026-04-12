using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public ItemSlot[] slots;

    public void CheckAnswer()
    {
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
    }

    void WrongAnswer()
    {
        Debug.Log("Type mismatch detected.");
        // Worker freeze animation
    }
}